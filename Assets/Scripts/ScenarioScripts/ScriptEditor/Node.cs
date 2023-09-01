using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public const int maxBranchCount = 4;

    private NodeGraph nodeGrp;

    public ScriptObject script;

    public NodeType nodeType;

    public RectTransform rectTransform;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImg;
    [SerializeField] private Text text;
    [SerializeField] private Button addBranchBtn;

    public Node prevNode = null;
    public Node nextNode = null;

    public Node parent = null;
    public List<Node> branch;

    public bool isHead
    {
        get
        {
            if(parent == null)
            {
                if(nodeGrp.head == this)
                {
                    return true;
                }
            }
            else
            {
                foreach(var branch in parent.branch)
                {
                    if(branch == this)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }


    private void Awake()
    {
        branch = new List<Node>(new Node[maxBranchCount]);
    }

    private void Start()
    {
        nodeGrp = NodeGraph.Instance;

        button.onClick.AddListener(() => nodeGrp.OnNodeClick(this));

        addBranchBtn.onClick.RemoveAllListeners();
        addBranchBtn.onClick.AddListener(() => {
            nodeGrp.SelectNode(this);
            nodeGrp.CreateBranch();
            Refresh();
        });
    }

    public void Init(ScriptObject script)
    {
        if(nodeGrp == null)
        {
            nodeGrp = NodeGraph.Instance;
        }

        transform.SetParent(nodeGrp.transform, false);

        this.script = script.Clone();

        Refresh();
    }

    public void SetDefaultParam()
    {
        if(script.scriptType != ScriptType.Event)
        {
            return;
        }

        var eventInfo = ScriptInfo.eventInfos[script.eventData.eventType];
        
        for(int i = 0; i < eventInfo.paramInfo.Count; ++i)
        {
            script.SetVariable(eventInfo.paramInfo[i].targetKey, eventInfo.paramInfo[i].defaultValue);
        }
    }

    public void Refresh()
    {
        RefreshNodeType();

        if(nodeType == NodeType.Branch && GetBranchCount() < maxBranchCount)
        {
            addBranchBtn.gameObject.SetActive(true);
        }
        else
        {
            addBranchBtn.gameObject.SetActive(false);
        }

        if (script.scriptType == ScriptType.Event)
        {
            buttonImg.color = Color.gray;
        }
        else
        {
            buttonImg.color = Color.white;
        }

        if(prevNode != null)
        {
            if(prevNode.script.linkEvent == true)
            {
                rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x * 0.6f);
            }
            else
            {
                rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x);
            }
        }
    }

    public void RefreshNodeType()
    {
        nodeType = GetNodeType(script);
    }

    public void SetName(string name)
    {
        this.name = name;
        this.text.text = name;
    }

    public void SelectNode()
    {
        buttonImg.color = Color.cyan;
    }

    public void DeselectNode()
    {
        if (script.scriptType == ScriptType.Event)
        {
            buttonImg.color = Color.gray;
        }
        else
        {
            buttonImg.color = Color.white;
        }
    }

    public void SetNextNode(Node node)
    {
        if (nextNode == null)
        {
            nextNode = node;
            node.prevNode = this;
        }
        else
        {
            Node temp = nextNode;

            nextNode = node;
            node.prevNode = this;

            nextNode.nextNode = temp;

            if (temp != null)
            {
                temp.prevNode = nextNode;
            }
        }
    }

    public void SetPrevNode(Node node)
    {
        if (prevNode == null)
        {
            prevNode = node;
            node.nextNode = this;
        }
        else
        {
            Node temp = prevNode;

            prevNode = node;
            node.nextNode = this;

            prevNode.prevNode = temp;

            if (temp != null)
            {
                temp.nextNode = prevNode;
            }
        }
    }

    public void SetNodePointerForDelete()
    {
        if(nextNode != null)
        {
            nextNode.prevNode = prevNode ?? null;
        }

        if(prevNode != null)
        {
            prevNode.nextNode = nextNode ?? null;
        }
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

    public int GetBranchIndex()
    {
        if (parent == null) { return -1; }

        Node head = null;

        for (Node node = this; node != null; node = node.prevNode)
        {
            head = node;
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

    public static NodeType GetNodeType(in ScriptObject script)
    {
        NodeType nodeType = NodeType.Normal;

        if (script.scriptType == ScriptType.Event)
        {
            if (script.eventData.eventType == EventType.Branch || script.eventData.eventType == EventType.Choice)
            {
                nodeType = NodeType.Branch;
            }
            else if (script.eventData.eventType == EventType.Goto)
            {
                nodeType = NodeType.Goto;
            }
            else if(script.eventData.eventType == EventType.BranchEnd)
            {
                nodeType = NodeType.BranchEnd;
            }
        }

        return nodeType;
    }
}
