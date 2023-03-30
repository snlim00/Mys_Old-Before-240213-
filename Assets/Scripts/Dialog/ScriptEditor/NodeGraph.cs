using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        //HideInspector
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.H))
            .Subscribe(_ =>
            {
                if(ScriptInspector.instance.gameObject.activeSelf == true)
                {
                    ScriptInspector.instance.gameObject.SetActive(false);
                    editorMgr.grpahPanel.SetActive(false);
                }
                else
                {
                    ScriptInspector.instance.gameObject.SetActive(true);
                    editorMgr.grpahPanel.SetActive(true);
                }
            });

        //Play
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.P))
            .Subscribe(_ =>
            {
                if (ScriptInspector.instance.gameObject.activeSelf == true)
                {
                    ScriptInspector.instance.gameObject.SetActive(false);
                    editorMgr.grpahPanel.SetActive(false);

                    Save();
                    DialogManager.instance.ReadScript(editorMgr.scriptGroupID);
                    DialogManager.instance.ExecuteMoveTo(selectedNode.script.scriptID, DialogManager.instance.DialogStart);
                }
                else
                {
                    ScriptInspector.instance.gameObject.SetActive(true);
                    editorMgr.grpahPanel.SetActive(true);

                    DialogManager.instance.StopDialog();
                }
            });


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
            .Where(_ => Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl))
            .Subscribe(_ => CreateNextNode());

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Delete))
            .Subscribe(_ => RemoveNode());

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))
            .Subscribe(_ => Save());

        Observable.EveryUpdate()
           .Where(_ => Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftControl))
           .Subscribe(_ =>
           {
               if(selectedNode.nodeType == Node.NodeType.BranchEnd)
               {
                   return;
               }

               if (selectedNode.script.scriptType == ScriptType.Text)
               {
                   selectedNode.script.scriptType = ScriptType.Event;
               }
               else
               {
                   selectedNode.script.scriptType = ScriptType.Text;
               }

               RefreshInspector();
               selectedNode.RefreshNodeType();
           });
    }

    public void RefreshInspector()
    {
        ScriptInspector.instance.ApplyInspector();
        ScriptInspector.instance.SetInspector(selectedNode);  
    }

    public void Save()
    {
        RefreshInspector();

        ScriptExport();
        "Save".Log();
    }

    private void ScriptExport()
    {
        RefreshAllNode();

        string path = (Application.dataPath + "/Data/ScriptTable" + editorMgr.scriptGroupID + ".CSV");

        using (var writer = new CsvFileWriter(path))
        {
            List<string> colums = new List<string>();
            string[] keys = Enum.GetNames(typeof(ScriptDataKey));

            colums.AddRange(keys);
            writer.WriteRow(colums);
            colums.Clear();

            TraversalNode(true, this.head, (index, branchIndex, depth, node) =>
            {
                //branch 파라미터 처리
                if(node.script.eventData.eventType == EventType.Branch)
                {
                    for(int i = 0; i < Node.maxBranchCount; ++i)
                    {
                        //브랜치가 없으면 해당 부분의 eventParam 비우기
                        if (node.branch[i] == null)
                        {
                            for(int j = i; j < Node.maxBranchCount; ++j)
                            {
                                node.script.eventData.eventParam[i * 2 + 1] = null;
                                node.script.eventData.eventParam[i * 2 + 2] = null;
                            }

                            break;
                        }

                        node.script.eventData.eventParam[i * 2 + 2] = node.branch[i].script.scriptID.ToString();
                    }
                }

                //값 등록
                foreach (ScriptDataKey key in Enum.GetValues(typeof(ScriptDataKey)))
                {
                    string value = node.script.GetVariableFromKey(key) ?? "";

                    colums.Add(value);
                }

                writer.WriteRow(colums);
                colums.Clear();
            });
        } 
    }

    public void CreateGraph()
    {
        int scriptGroupID = editorMgr.scriptGroupID;

        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.transform.SetParent(transform);
        node.transform.localScale = Vector3.one;
        node.transform.localPosition = Vector3.zero;

        head = node;

        SelectNode(node);

        RefreshAllNode();
    }

    public Node CreateNode()
    {
        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.transform.SetParent(transform);
        node.transform.localScale = Vector3.one;

        return node;
    }

    #region commands
    public void ExecuteCommand(EditorCommand cmd)
    {
        cmd.Execute();
        commands.Push(cmd);
    }

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

        if (selectedNode.GetBranchCount() >= Node.maxBranchCount)
        {
            return;
        }

        EditorCommand command = new CreateBranchNode();
        ExecuteCommand(command);
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
        ExecuteCommand(command);
    }

    public void RemoveNode()
    {
        if (selectedNode.nextNode == null && selectedNode.prevNode == null) //마지막 하나 남은 노드라면 지우지 않음
        {
            "마지막 하나 남은 노드는 삭제할 수 없습니다".LogWarning();
            return;
        }

        //이전 노드가 Branch이며 해당 노드의 다음 노드가 없다면 삭제할 수 없음.
        if (selectedNode.prevNode != null && selectedNode.prevNode.nodeType == Node.NodeType.Branch && selectedNode.nextNode == null)
        {
            "브랜치의 다음 노드는 항상 존재해야 합니다.".LogWarning();
            return;
        }

        if(selectedNode.parent != null)
        {
            if (selectedNode.prevNode == null && selectedNode.nextNode.nodeType == Node.NodeType.BranchEnd)
            {
                "브랜치의 마지막 하나 남은 노드는 삭제할 수 없습니다.".LogWarning();
                return;
            }
            else if(selectedNode.nodeType == Node.NodeType.BranchEnd)
            {
                EditorCommand cmd = new RemoveBranch();
                ExecuteCommand(cmd);
                return;
            }
        }

        EditorCommand command = new RemoveNode();
        ExecuteCommand(command);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="includeBranch"></param>
    /// <param name="action">index, branchIndex, depth, Node<br></br><br></br>
    /// index : 부모가 없는 노드들만 나열했을 때, 해당 노드의 번호<br></br>
    /// branchIndex : 해당 브랜치가 속한 부모의 branch의 index. 부모가 없다면 null<br></br>
    /// depth : 해당 브랜치에 속한 노드들만 나열했을 때, 해당 노드의 해당 브랜치에서의 번호.
    /// </param>
    /// <returns></returns>
    public int TraversalNode(bool includeBranch, Node head, Action<int, int?, int, Node> action, int index = 0)
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

            if(includeBranch == true)
            {
                if (node.branch.Count > 0)
                {
                    foreach (var branch in node.branch)
                    {
                        index = TraversalNode(true, branch, action, index);
                    }
                }
            }

            node = node.nextNode;
        }

        return index;
    }

    public int GetNodeCount()
    {
        return TraversalNode(true, head, null);
    }

    public void SetScriptID()
    {
        TraversalNode(true, head, (index, branchIndex, depth, node) =>
        {
            node.SetScriptID(index);
        });
    }

    public void RefreshAllNode()
    {
        SetScriptID();

        int loopCount = TraversalNode(true, head, (index, branchIndex, depth, node) =>
        {
            //노드 이름 설정
            {
                string name = "";
                
                if (node.parent == null)
                {
                    name += depth.ToString();
                }
                else
                {
                    if (node.isHead == true)
                    {
                        name += node.parent.name;
                        name += " - " + branchIndex;
                    }
                    else
                    {
                        name += depth;
                    }
                }

                node.SetName(name);
            }

            //노드 포지션 설정
            {
                if (node.isHead == true)
                {
                    if (node.parent == null)
                    {
                        node.transform.localPosition = Vector2.zero;
                    }
                    else
                    {
                        Vector2 pos = node.parent.transform.localPosition;
                        pos.y += Node.interval.y;
                        pos.x += (Node.interval.x * (branchIndex + 1) ?? 1) - Node.interval.x * 0.5f;

                        node.transform.localPosition = pos;
                    }
                }
                else if (node.prevNode != null)
                {

                    Vector2 pos = node.prevNode.transform.localPosition;

                    int childCount = node.prevNode.GetChildCount() + 1;
                    pos.y += Node.interval.y * childCount;
                    node.transform.localPosition = pos;
                }
            }

            //노드 리프레시
            {
                node.Refresh();

                //BranchEnd 처리
                if (node.nodeType == Node.NodeType.BranchEnd)
                {
                    node.SetName("-");

                    node.script.eventData.eventParam[0] = node?.parent?.nextNode.script.scriptID.ToString(); 
                    //다음 노드의 ScriptID가 refresh 되기 전임.. ScriptID를 다른 시점에서 처리해야 할 듯
                    //SetScriptID 함수를 따로 만들어서 이전에 실행하도록 하여 해결 230316
                }
            }
        });

        RefreshContentSize();
    }

    public void RefreshContentSize()
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
        if(node == selectedNode)
        {
            return;
        }

        selectedNode?.SetColorDeselect();

        selectedNode = node;
        node.SetColorSelect();
        ScriptInspector.instance.SetInspector(node);
    }

    public void SelectLastNode()
    {
        Node lastNode = head;

        TraversalNode(false, head, (_, _, _, node) =>
        {
            lastNode = node;
        });

        SelectNode(lastNode);
    }
    #endregion
}
