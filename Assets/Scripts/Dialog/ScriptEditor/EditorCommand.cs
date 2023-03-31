using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EditorCommand
{
    protected NodeGraph nodeGrp;
    protected ScriptObject script = null;

    public EditorCommand()
    {
        this.nodeGrp = NodeGraph.instance;
    }

    //SetScript를 통해 Execute전에 스크립트를 설정, 설정한 스크립트가 있다면 Execute내에서 스크립트를 적용(ApplyScript)하도록 설정.
    public EditorCommand SetScript(ScriptObject script)
    {
        this.script = (ScriptObject)script.Clone();

        return this;
    }

    protected void ApplyScript(Node node)
    {
        if (script == null)
        {
            return;
        }

        node.script = script;

        node.RefreshNodeType();
    }

    public abstract void Execute();

    public abstract void Undo();
}

public class CreateNextNode : EditorCommand
{
    private Node createdNode = null;
    private Node prevSelectedNode;

    public override void Execute()
    {
        createdNode = nodeGrp.CreateNode();

        createdNode.parent = nodeGrp.selectedNode.parent;

        nodeGrp.selectedNode.SetNextNode(createdNode);


        prevSelectedNode = nodeGrp.selectedNode;

        ApplyScript(createdNode);
        nodeGrp.RefreshAllNode();
        nodeGrp.SelectNode(createdNode);
    }

    public override void Undo()
    {
        if (createdNode.nextNode != null)
        {
            prevSelectedNode = createdNode.nextNode;
        }
        else
        {
            prevSelectedNode = createdNode.prevNode;
        }

        if (createdNode.nextNode != null)
        {
            createdNode.nextNode.prevNode = createdNode.prevNode ?? null;
        }

        if (createdNode.prevNode != null)
        {
            createdNode.prevNode.nextNode = createdNode.nextNode ?? null;
        }

        nodeGrp.SelectNode(this.prevSelectedNode ?? nodeGrp.head);

        GameObject.Destroy(this.createdNode.gameObject);

        nodeGrp.RefreshAllNode();
    }

    public Node GetCreatedNode()
    {
        return createdNode;
    }
}

public class CreateBranchNode : EditorCommand
{
    private Node createdNode;
    private Node branchEnd;

    public override void Execute()
    {
        //다음 노드가 없다면 노드 생성
        if(nodeGrp.selectedNode.nextNode == null)
        {
            Node prevSelectedNode = nodeGrp.selectedNode;

            EditorCommand command = new CreateNextNode();

            nodeGrp.ExecuteCommand(command);

            nodeGrp.SelectNode(prevSelectedNode);
        }

        //노드 생성
        {
            createdNode = nodeGrp.CreateNode();

            int i;
            for (i = 0; i < Node.maxBranchCount; ++i)
            {
                if (nodeGrp.selectedNode.branch[i] == null)
                {
                    break;
                }
            }
            
            nodeGrp.selectedNode.branch[i] = createdNode;
            createdNode.parent = nodeGrp.selectedNode;
        }
        

        //BranchEnd 생성
        {
            branchEnd = nodeGrp.CreateNode();
            branchEnd.SetPrevNode(createdNode);
            branchEnd.SetNodeType(Node.NodeType.BranchEnd);

            branchEnd.parent = createdNode.parent;

            branchEnd.script.scriptType = ScriptType.Event;
            branchEnd.script.eventData.eventType = EventType.Goto;
        }

        ApplyScript(createdNode);
        nodeGrp.RefreshAllNode();
        nodeGrp.SelectNode(createdNode);
    }

    public override void Undo()
    {
        nodeGrp.SelectNode(nodeGrp.head);

        int branchIndex = createdNode.GetBranchIndex();

        createdNode.parent.branch.RemoveAt(branchIndex);
        createdNode.parent.branch.Add(null);

        GameObject.Destroy(createdNode.gameObject);
        GameObject.Destroy(branchEnd.gameObject);

        nodeGrp.RefreshAllNode();
    }
}

public class RemoveNode : EditorCommand
{
    private Node removedNode;

    ~RemoveNode() //Destroy가 안됨 ,. . . . . ..ㅠㅠㅠㅠ
    {
        "소멸".로그();

        try
        {
            GameObject.Destroy(removedNode.gameObject);
        }
        catch
        {
            "오류가 나긴 났음".로그();
        }
    }

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
            removedNode.nextNode.prevNode = removedNode.prevNode ?? null;
        }

        if(removedNode.prevNode != null)
        {
            removedNode.prevNode.nextNode = removedNode.nextNode ?? null;
        }

        if (removedNode.isHead == true)
        {
            if(removedNode.parent == null)
            {
                nodeGrp.head = newSelectedNode;
            }
            else
            {
                int idx = removedNode.GetBranchIndex();

                removedNode.parent.branch[idx] = newSelectedNode;
            }
            //"change first node".Log();
            
        }

        removedNode.transform.SetParent(null);

        nodeGrp.RefreshAllNode();

        nodeGrp.SelectNode(newSelectedNode);
    }

    public override void Undo()
    {
        removedNode.transform.SetParent(nodeGrp.transform);

        if(removedNode.nextNode != null)
        {
            removedNode.nextNode.prevNode = removedNode;
        }

        if(removedNode.prevNode != null)
        {
            removedNode.prevNode.nextNode = removedNode;
        }

        nodeGrp.RefreshAllNode();
        nodeGrp.SelectNode(removedNode);
    }
}

public class RemoveBranch : EditorCommand
{
    int index;
    private Node removedBranchHead;
    private List<Node> removedBranch;

    public override void Execute()
    {
        removedBranch = new();

        Node parent = nodeGrp.selectedNode.parent;
        index = nodeGrp.selectedNode.GetBranchIndex();

        removedBranchHead = parent.branch[index];

        nodeGrp.TraversalNode(false, removedBranchHead, (index, branchIndex, depth, node) =>
        {
            removedBranch.Add(node);
            node.transform.SetParent(null);
        });

        parent.branch.RemoveAt(index);
        parent.branch.Add(null);

        nodeGrp.RefreshAllNode();
        nodeGrp.SelectNode(removedBranchHead.parent);
    }

    public override void Undo()
    {
        removedBranchHead.parent.branch.Insert(index, removedBranchHead);
        removedBranchHead.parent.branch.RemoveAt(Node.maxBranchCount);

        foreach(Node node in removedBranch)
        {
            node.transform.SetParent(nodeGrp.transform);
        }

        nodeGrp.RefreshAllNode();
    }
}