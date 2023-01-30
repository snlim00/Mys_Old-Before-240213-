using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class NodeGraph : MonoBehaviour
{
    public static NodeGraph instance = null;

    [SerializeField] private EditorManager editorMgr;

    public Node firstNode;
    public List<Node> nodeList;

    [SerializeField] private GameObject nodePref;

    public Node selectedNode = null;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        editorMgr = EditorManager.instance;

        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.C) && Input.GetMouseButtonDown(0))
            .Subscribe(_ => CreateNextNode());
    }

    public void LoadGraph(string path)
    {
        GameObject graphPref = Resources.Load<GameObject>(path);

        GameObject graph = Instantiate(graphPref);
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

    public void SetNodePosition()
    {
        Node node = firstNode;
        int loopCount = 1;

        while(node.nextNode != null)
        {
            loopCount.Log();

            node = node.nextNode;
            ++loopCount;
            node.SetScriptID(loopCount);


            Vector2 pos = node.prevNode.transform.localPosition;
            pos.y += Node.interval;
            pos.Log();
            node.transform.localPosition = pos;
        }

        var rect = GetComponent<RectTransform>();

        Vector2 sizeDelta = rect.sizeDelta;
        sizeDelta.y = loopCount * Mathf.Abs(Node.interval) + 100;
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
    }
    #endregion
}
