using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public enum NodeType
{
    Text,
    Event,
}

public class Node : MonoBehaviour
{
    public readonly Color selectedColor = new Color32(114, 134, 211, 255);
    public readonly Color subSelectedColor = new Color32(142, 162, 233, 255);
    public const float interval = -30;

    private NodeGraph nodeGraph;

    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text text;
    [SerializeField] private UILineRenderer lineRdr;

    public Node prevNode = null;
    public Node nextNode = null;

    public ScriptObject script = new();

    public NodeType nodeType = NodeType.Text;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
        nodeGraph = NodeGraph.instance;
    }

    private void OnButtonClick()
    {
        nodeGraph.SelectNode(this);
    }

    public void SetScriptID(int id)
    {
        script.scriptID = EditorManager.instance.scriptGroupID * 10000 + id;

        text.text = id.ToString();
    }

    public void SetNextNode(Node node)
    {
        Node oldNode = null;
        if(nextNode != null)
        {
            oldNode = nextNode;
        }

        this.nextNode = node;
        node.prevNode = this;
        node.nextNode = oldNode;

        if(oldNode != null)
        {
            oldNode.prevNode = node;
        }
    }

    public void SetPrevNode(Node node)
    {
        Node oldNode = null;
        if (nextNode != null)
        {
            oldNode = prevNode;
        }

        this.prevNode = node;
        node.nextNode = this;
        node.prevNode = oldNode;

        if(oldNode != null)
        {
            oldNode.nextNode = node;
        }
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
