using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NodeGraph : MonoBehaviour
{
    public static NodeGraph instance = null;

    private EditorManager editorMgr;

    public Node firstNode;

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
            .Where(_ => Input.GetKeyDown(KeyCode.Delete))
            .Subscribe(_ => RemoveNode());

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ => Save());

        Observable.EveryUpdate()
           .Where(_ => Input.GetKey(KeyCode.E))
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

        firstNode = node;

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

    public void CreateNextNode()
    {
        if (selectedNode.nodeType == Node.NodeType.Goto)
        {
            return;
        }


        EditorCommand command = new CreateNextNode();
        command.Execute();
        commands.Push(command);
    }

    public void RemoveNode()
    {
        int nodeCount = GetNodeCount();

        if(nodeCount == 1) //마지막 하나 남은 노드라면 지우지 않음
        {
            return;
        }

        EditorCommand command = new RemoveNode();
        command.Execute();
        commands.Push(command);
    }

    public int DoAllNode(bool includeBranch, Action<int, Node> action)
    {
        Node node = firstNode;
        int loopCount = 0;

        while (node != null)
        {
            ++loopCount;

            if(action != null)
            {
                action(loopCount, node);
            }

            node = node.nextNode;
        }

        return loopCount;
    }

    public int GetNodeCount()
    {
        return DoAllNode(true, null);
    }

    public void SetNodePosition()
    {
        int loopCount = DoAllNode(true, (loopCount, node) =>
        {
            node.SetScriptID(loopCount);
            node.name = loopCount.ToString();

            if(node.prevNode != null)
            {
                Vector2 pos = node.prevNode.transform.localPosition;
                pos.y += Node.interval;
                node.transform.localPosition = pos;
            }
        });
    }

    public void SetContentSize()
    {
        int nodeCount = GetNodeCount();

        Vector2 sizeDelta = rect.sizeDelta;
        sizeDelta.y = nodeCount * Mathf.Abs(Node.interval) + 100;
        rect.sizeDelta = sizeDelta;

        editorMgr.scrollViewContent.sizeDelta = new Vector2(editorMgr.scrollViewContent.sizeDelta.x, sizeDelta.y);
    }

    #region 노드 선택
    public void SelectNode(Node node)
    {
        //Save();

        if (selectedNode != null)
        {
            selectedNode.Deselect();
        }

        selectedNode = node;
        node.Select();
        ScriptInspector.instance.SetInspector(node);
    }
    #endregion
}
