using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorCommand
{
    protected NodeGraph nodeGrp;
    protected ScriptObject script = null;

    public EditorCommand()
    {
        this.nodeGrp = NodeGraph.Instance;
    }

    public EditorCommand SetScript(ScriptObject script)
    {
        this.script = script;

        return this;
    }

    protected void ApplyScript(Node node)
    {
        if(script == null)
        {
            return;
        }

        node.Init(script);

        node.RefreshNodeType();
    }

    public abstract void Execute();

    public abstract void Undo();
}

public class CreateNextNode : EditorCommand
{
    public Node createdNode { get; private set; } = null;
    private Node prevSelectedNode;

    public override void Execute()
    {
        createdNode = nodeGrp.CreateNode();

        createdNode.parent = nodeGrp.selectedNode.parent;
        
        nodeGrp.selectedNode.SetNextNode(createdNode);

        prevSelectedNode = nodeGrp.selectedNode;

        ApplyScript(createdNode);
        nodeGrp.RefreshAll();
        nodeGrp.SelectNode(createdNode);
    }

    public override void Undo()
    {
        createdNode.SetNodePointerForDelete();

        nodeGrp.SelectNode(createdNode.prevNode ?? createdNode.nextNode ?? prevSelectedNode ?? nodeGrp.head);

        GameObject.Destroy(this.createdNode.gameObject);

        nodeGrp.RefreshAll();
    }
}

public class RemoveNode : EditorCommand
{
    private Node removedNode;
    private bool isHead = false; //삭제하는 노드가 헤드였다면 해당 변수 true, 언도 시 head여부에 따라 NodeGrpah의 Head를 removedNode로 변경해 줌.
    private int branchIndex = -1;

    public override void Execute()
    {
        removedNode = nodeGrp.selectedNode;

        Node newSelectedNode = removedNode.nextNode ?? removedNode.prevNode ?? nodeGrp.head;

        removedNode.SetNodePointerForDelete();

        isHead = removedNode.isHead;

        if(isHead == true)
        {
            if(removedNode.parent == null)
            {
                nodeGrp.head = newSelectedNode;
            }
            else //브랜치의 head가 삭제되는 거라면 parent의 branch도 변경.
            {
                int idx = removedNode.GetBranchIndex();

                removedNode.parent.branch[idx] = newSelectedNode;

                branchIndex = idx;
            }
        }

        removedNode.transform.SetParent(null, false);

        nodeGrp.SelectNode(newSelectedNode);

        nodeGrp.RefreshAll();
    }

    public override void Undo()
    {
        removedNode.transform.SetParent(nodeGrp.transform);

        removedNode.prevNode?.SetNextNode(removedNode);
        removedNode.nextNode?.SetPrevNode(removedNode);

        if(isHead == true)
        {
            if(branchIndex == -1)
            {
                nodeGrp.head = removedNode;
            }
            else
            {
                removedNode.parent.branch[branchIndex] = removedNode;
            }
        }

        nodeGrp.SelectNode(removedNode);
        nodeGrp.RefreshAll();
    }
}

public class CreateBranch : EditorCommand
{
    public bool doCreateNextNode = true;
    public bool doCreateBranchEnd = true;

    public Node createdNode { get; private set; } = null;
    public Node branchEnd { get; private set; } = null;
    public Node nextNode { get; private set; } = null;

    public override void Execute()
    {
        //다음 노드가 없다면 노드 생성
        if(nodeGrp.selectedNode.nextNode == null && doCreateNextNode == true)
        {
            Node prevSelectedNode = nodeGrp.selectedNode;

            var cmd = new CreateNextNode();
            cmd.Execute();
            nextNode = cmd.createdNode;

            nodeGrp.SelectNode(prevSelectedNode);
        }

        //노드 생성
        {
            createdNode = nodeGrp.CreateNode();

            int i = 0;
            for(i = 0; i < Node.maxBranchCount; ++i)
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
        if(doCreateBranchEnd == true)
        {
            branchEnd = nodeGrp.CreateNode();
            branchEnd.SetPrevNode(createdNode);

            branchEnd.parent = createdNode.parent;

            branchEnd.script.scriptType = ScriptType.Event;
            branchEnd.script.eventData.eventType = EventType.BranchEnd;
        }

        ApplyScript(createdNode);
        nodeGrp.SelectNode(createdNode);
        nodeGrp.RefreshAll();
    }

    public override void Undo()
    {

    }
}