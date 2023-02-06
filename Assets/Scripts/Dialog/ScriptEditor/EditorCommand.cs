using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IEditorCommand
{
    public void Execute();

    public void Undo();
}

public class CreateNextNode : IEditorCommand
{
    private NodeGraph nodeGrp;

    private Node createdNode;
    private Node prevSelectedNode;

    public CreateNextNode()
    {
        nodeGrp = NodeGraph.instance;
    }

    public void Execute()
    {
        createdNode = nodeGrp.CreateNode();

        nodeGrp.selectedNode.SetNextNode(createdNode);

        prevSelectedNode = nodeGrp.selectedNode;

        nodeGrp.SetNodePosition();
        nodeGrp.SetContentSize();
        nodeGrp.SelectNode(createdNode);
    }

    public void Undo()
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

        GameObject.Destroy(createdNode);
    }
}