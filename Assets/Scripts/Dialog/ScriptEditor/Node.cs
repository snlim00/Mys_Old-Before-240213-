using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Node : MonoBehaviour
{
    public enum ScriptType
    {
        Text,
        Event,
    }

    public enum NodeType
    {
        Normal,
        Branch,
        BranchEnd,
        Goto,
    }

    public static readonly Color selectedColor = new Color32(114, 134, 211, 255);
    //public static readonly Color subSelectedColor = new Color32(142, 162, 233, 255);
    public static readonly Vector2 interval = new Vector2(67, -30);
    public const int maxBranchCount = 3;

    private NodeGraph nodeGrp;

    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text text;

    public Node prevNode = null;
    public Node nextNode = null;

    public ScriptObject script = new();

    public ScriptType scriptType = ScriptType.Text;

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
                    if (branch == this) return true;
                }
            }

            return false;
        }
    }

    public NodeType nodeType
    {
        get
        {
            if(scriptType == ScriptType.Text)
            {
                return NodeType.Normal;
            }
            else
            {
                switch (script.eventData.eventType)
                {
                    case EventType.Branch:
                        return NodeType.Branch;

                    case EventType.Goto:
                        if(GetBranchIndex() == -1)
                        {
                            return NodeType.Goto;
                        }
                        else
                        {
                            return NodeType.BranchEnd;
                        }
                }
            }

            return NodeType.Normal;
        }
    }

    private void Awake()
    { 
        nodeGrp = NodeGraph.instance;

        branch = new(new Node[maxBranchCount]);
    }

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        nodeGrp.SelectNode(this);
    }

    public void SetScriptID(int id)
    {
        script.scriptID = EditorManager.instance.scriptGroupID * 10000 + id;
    }

    public void SetText(string text)
    {
        this.text.text = text;
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

    /// <summary>
    /// 브랜치의 인덱스를 반환합니다. 브랜치에 속하지 않았다면 -1을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int GetBranchIndex()
    {
        if (isHead == false || parent == null)
        {
            return -1;
        }

        for(int i = 0; i < parent.branch.Count; ++i)
        {
            if (parent.branch[i] == this)
            {
                return i;
            }
        }

        return -1;
    }

    public void Select()
    {
        buttonImage.color = selectedColor;
    }

    public void Deselect()
    {
        buttonImage.color = Color.white;
    }
}
