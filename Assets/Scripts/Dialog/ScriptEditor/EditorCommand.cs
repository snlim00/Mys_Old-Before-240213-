using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EditorCommand
{
    protected NodeGraph nodeGrp;

    public EditorCommand()
    {
        this.nodeGrp = NodeGraph.instance;
    }

    public abstract void Execute();

    public abstract void Undo();
}

public class CreateNextNode : EditorCommand
{
    private Node createdNode;
    private Node prevSelectedNode;

    public override void Execute()
    {
        createdNode = nodeGrp.CreateNode();

        nodeGrp.selectedNode.SetNextNode(createdNode);

        prevSelectedNode = nodeGrp.selectedNode;

        nodeGrp.SetNodePosition();
        nodeGrp.SetContentSize();
        nodeGrp.SelectNode(createdNode);
    }

    public override void Undo()
    {
        if(prevSelectedNode != null)
        {
            nodeGrp.SelectNode(prevSelectedNode);
        }
        else
        {
            if(createdNode.prevNode != null)
            {
                nodeGrp.SelectNode(createdNode.prevNode);
            }
            else
            {
                nodeGrp.SelectNode(createdNode.nextNode);
            }
        }

        GameObject.Destroy(createdNode.gameObject);

        nodeGrp.SetNodePosition();
        nodeGrp.SetContentSize();
    }
}

public class RemoveNode : EditorCommand
{
    private Node removedNode;

    public override void Execute()
    {
        Node newSelectedNode;

        removedNode = nodeGrp.selectedNode;

        if (removedNode.nextNode != null)
        {
            newSelectedNode = removedNode.nextNode;
        }
        else
        {
            newSelectedNode = removedNode.prevNode;
        }

        if (removedNode.nextNode != null)
        {
            if(removedNode.prevNode != null)
            { 
                removedNode.nextNode.prevNode = removedNode.prevNode;
            }
        }

        if(removedNode.prevNode != null)
        {
            if(removedNode.nextNode != null)
            {
                removedNode.prevNode.nextNode = removedNode.nextNode;
            }
        }

        if (removedNode == nodeGrp.firstNode)
        {
            "change first node".Log();
            nodeGrp.firstNode = newSelectedNode;
        }

        removedNode.transform.SetParent(null);

        nodeGrp.SetNodePosition();

        nodeGrp.SetContentSize();

        nodeGrp.SelectNode(newSelectedNode);
    }

    public override void Undo()
    {
        removedNode.transform.SetParent(nodeGrp.transform);

        if(removedNode.nextNode != null)
        {
            "1".Log();
            removedNode.nextNode.prevNode = removedNode;
        }

        if(removedNode.prevNode != null)
        {
            "2".Log();
            removedNode.prevNode.nextNode = removedNode;
        }

        nodeGrp.SetNodePosition();
        nodeGrp.SetContentSize();
        nodeGrp.SelectNode(removedNode);
    }
}