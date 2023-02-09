using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class NodeGraph : MonoBehaviour
{
    public static NodeGraph instance = null;

    private EditorManager editorMgr;

    public Node head;

    private GameObject nodePref;

    public Node selectedNode = null;

    private RectTransform rect;

    private Stack<EditorCommand> commands = new(); //실행된 커맨드
    private Stack<EditorCommand> redoCommands = new(); //언도된 커맨드

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        editorMgr = EditorManager.instance;

        nodePref = Resources.Load<GameObject>("Prefabs/ScriptEditor/Node");
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            .Subscribe(_ =>
            {
                EditorCommand cmd;
                
                if(commands.TryPop(out cmd) == true)
                {
                    cmd.Undo();
                    redoCommands.Push(cmd);
                }
            });

        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
            .Subscribe(_ =>
            {
                EditorCommand cmd;

                if (redoCommands.TryPop(out cmd) == true)
                {
                    cmd.Execute();
                    commands.Push(cmd);
                }
            });

        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.C) && Input.GetMouseButtonDown(0))
            .Subscribe(_ => CreateNextNode());

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.B))
            .Subscribe(_ => CreateBranch());

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Delete))
            .Subscribe(_ => RemoveNode());

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ => Save());

        Observable.EveryUpdate()
           .Where(_ => Input.GetKeyDown(KeyCode.E))
           .Subscribe(_ =>
           {
               if (selectedNode.scriptType == Node.ScriptType.Text)
               {
                   selectedNode.scriptType = Node.ScriptType.Event;
               }
               else
               {
                   selectedNode.scriptType = Node.ScriptType.Text;
               }

               Save();
           });


        SetNodePosition();
        SetContentSize();
        SelectNode(selectedNode ?? head);
    }

    public void Save()
    {
        ScriptInspector.instance.ApplyInspector();
        ScriptInspector.instance.SetInspector(selectedNode);

        PrefabUtility.SaveAsPrefabAsset(this.gameObject, Application.dataPath + "/Resources/Prefabs/ScriptGraph/ScriptGraph" + editorMgr.scriptGroupID + ".prefab");
        "Save".Log();
    }

    public void CreateGraph()
    {
        int scriptGroupID = editorMgr.scriptGroupID;

        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.transform.SetParent(transform);
        node.transform.localScale = Vector3.one;
        node.transform.localPosition = Vector3.zero;

        head = node;

        if (selectedNode == null)
        {
            SelectNode(node);
        }
    }

    public Node CreateNode()
    {
        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.transform.SetParent(transform);
        node.transform.localScale = Vector3.one;

        return node;
    }

    #region commands
    public void CreateBranch()
    {
        if (selectedNode.nodeType == Node.NodeType.Goto)
        {
            return;
        }

        if (selectedNode.nodeType == Node.NodeType.BranchEnd)
        {
            return;
        }

        EditorCommand command = new CreateBranchNode();
        command.Execute();
        commands.Push(command);
    }

    public void CreateNextNode()
    {
        if (selectedNode.nodeType == Node.NodeType.Goto)
        {
            return;
        }

        if(selectedNode.nodeType == Node.NodeType.BranchEnd)
        {
            return;
        }

        EditorCommand command = new CreateNextNode();
        command.Execute();
        commands.Push(command);
    }

    public void RemoveNode()
    {
        selectedNode.nextNode.Log();
        selectedNode.prevNode.Log();
        if (selectedNode.nextNode == null && selectedNode.prevNode == null) //마지막 하나 남은 노드라면 지우지 않음
        {
            return;
        }

        EditorCommand command = new RemoveNode();
        command.Execute();
        commands.Push(command);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="includeBranch"></param>
    /// <param name="action">index, branchIndex, depth, Node<br></br><br></br>
    /// index : 부모가 없는 노드들만 나열했을 때, 해당 노드의 번호<br></br>
    /// branchIndex : 해당 브랜치가 속한 부모의 branch의 index null<br></br>
    /// depth : 해당 브랜치에 속한 노드들만 나열했을 때, 해당 노드의 해당 브랜치에서의 번호. 부모가 없다면 null
    /// </param>
    /// <returns></returns>
    public int DoAllNode(bool includeBranch, Action<int, int?, int?, Node> action)
    {
        Node node = head;
        int index = 0;

        while (node != null)
        {
            index++;

            if (action != null)
            {
                action(index, null, null, node);
            }

            node = node.nextNode;
        }

        return index;
    }

    public int TraversalNode(Node head, Action<int, int?, int, Node> action, int index = 0)
    {
        Node node = head;
        int? branchIndex = null;
        int depth = 0;

        while(node != null)
        {
            ++index;
            ++depth;
            branchIndex = node.GetBranchIndex() != -1 ? node.GetBranchIndex() : null;

            if (action != null)
            {
                action(index, branchIndex, depth, node);
            }

            if (node.branch.Count > 0)
            {
                foreach (var branch in node.branch)
                {
                    index = TraversalNode(branch.Value, action, index);
                }
            }

            node = node.nextNode;
        }

        return index;
    }

    public int TraversalBranch(in Node parent, in int index, Action<int, int?, int?, Node> action)
    {
        int count = 1;

        return count;
    }

    public int GetNodeCount()
    {
        return DoAllNode(true, null);
    }

    public void SetNodePosition()
    {
        int loopCount = TraversalNode(head, (index, branchIndex, depth, node) =>
        {
            node.SetScriptID(index);

            string name = "";

            if(node.parent == null)
            {
                name += depth.ToString();
            }
            else
            {
                if (branchIndex != null)
                {
                    name += node.parent.name;
                    name += " : " + branchIndex;
                    name += " - " + depth;
                }
            }
            
            node.name = name;
            node.SetText(name);

            if (node.isHead == true)
            {
                if (node.parent == null)
                {
                    node.transform.localPosition = Vector2.zero;
                }
                else if (branchIndex != null && node.isHead == true)
                {
                    Vector2 parentPos = node.parent.transform.localPosition;
                    parentPos.y += Node.interval.y * 0.4f;
                    parentPos.x += Node.interval.x;

                    node.transform.localPosition = parentPos;
                }
            }
            else if(node.prevNode != null)
            {
                Vector2 pos = node.prevNode.transform.localPosition;
                pos.y += Node.interval.y;
                node.transform.localPosition = pos;
            }
        });
    }

    public void SetContentSize()
    {
        int nodeCount = GetNodeCount();

        Vector2 sizeDelta = rect.sizeDelta;
        sizeDelta.y = nodeCount * Mathf.Abs(Node.interval.y) + 100;
        rect.sizeDelta = sizeDelta;

        editorMgr.scrollViewContent.sizeDelta = new Vector2(editorMgr.scrollViewContent.sizeDelta.x, sizeDelta.y);
    }

    #region 노드 선택
    public void SelectNode(Node node)
    {
        if(node.nodeType == Node.NodeType.BranchEnd)
        {
            "브랜치의 끝엔 노드를 추가할 수 없습니다.".Log();
            return;
        }

        selectedNode?.Deselect();

        selectedNode = node;
        node.Select();
        ScriptInspector.instance.SetInspector(node);
    }
    #endregion
}
