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
    public static readonly Vector2 interval = new Vector2(75, -30);
    public const int maxBranchCount = 3;

    private NodeGraph nodeGrp;

    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text text;

    public Node prevNode = null;
    public Node nextNode = null;

    public ScriptObject script = new();

    public NodeType nodeType = NodeType.Normal;
    public ScriptType scriptType = ScriptType.Text;

    public Node parent = null;
    public Dictionary<int, Node> branch = new();

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
                    if(branch.Value == this)
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
        nodeGrp = NodeGraph.instance;
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

    public int GetBranchIndex()
    {
        if (isHead == false || parent == null)
        {
            return -1;
        }

        foreach (var branch in parent.branch)
        {
            if (branch.Value == this)
            {
                return branch.Key;
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
