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
    public List<Node> nodeList;

    private GameObject nodePref;

    public Node selectedNode = null;

    private RectTransform rect;

    private void Awake()
    {
        if(instance != null)
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
            .Where(_ => Input.GetKey(KeyCode.C) && Input.GetMouseButtonDown(0))
            .Subscribe(_ => CreateNextNode());

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ => Save());

        Observable.EveryUpdate()
           .Where(_ => Input.GetKey(KeyCode.E))
           .Subscribe(_ =>
           {
               if(selectedNode.scriptType == Node.ScriptType.Text)
               {
                   selectedNode.scriptType = Node.ScriptType.Event;
               }
               else
               {
                   selectedNode.scriptType = Node.ScriptType.Text;
               }

               Save();
           });


        SetContentSize();
    }

    public void Save()
    {
        ScriptInspector.instance.ApplyInspector();
        PrefabUtility.SaveAsPrefabAsset(this.gameObject, Application.dataPath + "/Resources/Prefabs/ScriptGraph/ScriptGraph" + editorMgr.scriptGroupID + ".prefab");
        "Save".Log();

        ScriptInspector.instance.SetInspector(selectedNode);
    }

    public void CreateGraph(EditorManager em)
    {
        //editorMgr = em;

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

        nodeList.Add(node);
    }
    
    public void CreateNextNode()
    {
        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.transform.SetParent(transform);
        node.transform.localScale = Vector3.one;

        selectedNode.SetNextNode(node);
        
        nodeList.Add(node);

        SetNodePosition();

        SelectNode(node);
    }

    public int DoAllNode(bool includeBranch, Action<int, Node> action)
    {
        Node node = firstNode;
        int loopCount = 0;

        while (true)
        {
            ++loopCount;

            if(action != null)
            {
                action(loopCount, node);
            }

            if(node.nextNode == null)
            {
                break;
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
