using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ObjectList : Singleton<ObjectList>
{
    [SerializeField] private GameObject objectPref;

    private NodeGraph nodeGrp;

    public const string unnamed = "! Unnamed !";

    private void Awake()
    {
        nodeGrp = NodeGraph.Instance;
    }

    public void RefreshList()
    {
        List<string> objectList = new();

        nodeGrp.TraversalNode(true, nodeGrp.head, (index, branchIndex, depth, node) =>
        {
            if(node.script.scriptType == ScriptType.Event)
            {
                if(node.script.eventData.eventType == EventType.CreateObject)
                {
                    objectList.Add(node.script.eventData.eventParam[0]);
                }
                else if(node.script.eventData.eventType == EventType.RemoveObject)
                {
                    objectList.Remove(node.script.eventData.eventParam[0]);
                }
                else if(node.script.eventData.eventType == EventType.RemoveAllObject)
                {
                    objectList = new();
                }
            }
        },
        stopCondition: (index, branchIndex, depth, node) =>
        {
            if(node.script.scriptId >= nodeGrp.selectedNode.script.scriptId)
            {
                return true;
            }
            else
            {
                return false;
            }
        });

        SetObjectList(objectList);
    }

    public void SetObjectList(List<string> objectList)
    {
        transform.DestroyAllChildren();

        foreach(var name in objectList)
        {
            CreateCharacterList(name);
        }
    }

    private Button CreateCharacterList(string name)
    {
        Button btn = Instantiate(objectPref).GetComponent<Button>();

        btn.transform.SetParent(this.transform, false);

        if(string.IsNullOrWhiteSpace(name))
        {
            btn.name = unnamed;
            btn.SetButtonText(unnamed);
        }
        else
        {
            btn.name = name;
            btn.SetButtonText(name);
        }

        btn.onClick.AddListener(() =>
        {
            if(nodeGrp.inputType == InputType.Object)
            {
                nodeGrp.InvokeSelectObject(btn.name);
                nodeGrp.CancelSelectObject();
            }
        });

        return btn;
    }
}
