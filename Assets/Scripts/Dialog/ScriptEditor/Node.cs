using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{


    public enum NodeType
    {
        Normal,
        Branch,
        BranchEnd,
        Goto,
    }

    public static readonly Color selectedColor = new Color32(114, 134, 211, 255);
    //public static readonly Color subSelectedColor = new Color32(142, 162, 233, 255);
    public static readonly Vector2 interval = new Vector2(82, -30);
    public const int maxBranchCount = 3;

    private NodeGraph nodeGrp;

    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text text;
    [SerializeField] private Button addBranchBtn;

    public Node prevNode = null;
    public Node nextNode = null;

    public ScriptObject script;

    public NodeType nodeType;

    public Node parent = null;
    public List<Node> branch;

    public bool isHead
    {
        get
        {
            if (parent == null)
            {
                if (nodeGrp.head == this)
                {
                    return true;
                }
            }
            else
            {
                foreach (var branch in parent.branch)
                {
                    if (branch == this) return true;
                }
            }

            return false;
        }
    }


    private void Awake()
    {
        nodeGrp = NodeGraph.instance;

        branch = new(new Node[maxBranchCount]);

        script = new();
    }

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);

        addBranchBtn.onClick.AddListener(OnBranchBtnClick);
    }

    private void OnButtonClick()
    {
        Select();
    }

    private void OnBranchBtnClick()
    {
        Select();

        nodeGrp.CreateBranch();

        RefreshBranchBtnActive();
    }

    public void Select()
    {
        nodeGrp.SelectNode(this);
    }

    public void SetScriptID(int id)
    {
        script.scriptID = EditorManager.instance.scriptGroupID * 10000 + id;
    }

    public void SetName(string text)
    {
        this.text.text = text;
        this.gameObject.name = text;
    }

    public void SetNextNode(Node node)
    {
        Node oldNode = null;
        if (nextNode != null)
        {
            oldNode = nextNode;
        }

        this.nextNode = node;

        if (node != null)
        {
            node.prevNode = this;
            node.nextNode = oldNode;
        }

        if(oldNode != null)
        {
            oldNode.prevNode = node;
        }
    }

    public void SetPrevNode(Node node)
    {
        Node oldNode = null;
        if (prevNode != null)
        {
            oldNode = prevNode;
        }

        this.prevNode = node;

        if(node != null)
        {
            node.nextNode = this;
            node.prevNode = oldNode;
        }

        if(oldNode != null)
        {
            oldNode.nextNode = node;
        }
    }

    public void SetNodeType(NodeType nodeType)
    {
        this.nodeType = nodeType;

        switch(nodeType)
        {
            case NodeType.Normal:
                break;

            case NodeType.Branch:
                break;

            case NodeType.BranchEnd:
                break;
        }

        RefreshBranchBtnActive();
    }

    public void Refresh()
    {
        RefreshNodeType();
        RefreshBranchBtnActive();
    }

    public void RefreshNodeType()
    {
        if (script.scriptType == ScriptType.Event)
        {
            if (script.eventData.eventType == EventType.Branch)
            {
                SetNodeType(NodeType.Branch);
            }
            else if (script.eventData.eventType == EventType.Goto)
            {
                if (parent == null)
                {
                    SetNodeType(NodeType.Goto);
                }
                else
                {
                    SetNodeType(NodeType.BranchEnd);
                }
            }
            else
            {
                SetNodeType(NodeType.Normal);
            }
        }
        else
        {
            SetNodeType(NodeType.Normal);
        }
    }

    public void RefreshBranchBtnActive()
    {
        if (nodeType != Node.NodeType.Branch)
        {
            HideBranchBtn();
            return;
        }

        if (GetBranchCount() >= maxBranchCount)
        {
            HideBranchBtn();
        }
        else
        {
            ShowBranchBtn();
        }
    }

    private void ShowBranchBtn()
    {
        addBranchBtn.gameObject.SetActive(true);
    }

    private void HideBranchBtn()
    {
        addBranchBtn.gameObject.SetActive(false);
    }

    public int GetChildCount()
    {
        int childCount = 0;

        for(int i = 0; i < branch.Count; ++i)
        {
            if (branch == null) break;

            int count = nodeGrp.TraversalNode(false, branch[i], null);

            if (count > childCount)
            {
                childCount = count;
            }
        }

        return childCount;
    }

    public int GetBranchCount()
    {
        for(int i = 0; i < branch.Count; ++i)
        {
            if (branch[i] == null)
            {
                return i;
            }
        }

        return maxBranchCount;
    }

    /// <summary>
    /// 브랜치의 인덱스를 반환합니다. 브랜치에 속하지 않았다면 -1을 반환합니다.
    /// </summary>
    public int GetBranchIndex()
    {
        if (parent == null)
        {
            return -1;
        }

        Node head = null;
        Node node = this;

        while(node != null)
        {
            head = node;

            node = node.prevNode;
        }

        if(head == null)
        {
            return -1;
        }

        for (int i = 0; i < parent.branch.Count; ++i)
        {
            if (parent.branch[i] == head)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetColorSelect()
    {
        buttonImage.color = selectedColor;
    }

    public void SetColorDeselect()
    {
        buttonImage.color = Color.white;
    }
}
