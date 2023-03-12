using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ScriptInspector : MonoBehaviour
{
    public static ScriptInspector instance = null;

    private GameObject variablePref;

    private List<Variable> variableList;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        variablePref = Resources.Load<GameObject>("Prefabs/ScriptEditor/Variable");
    }

    public void RefreshInspector(Node node)
    {
        ApplyInspector();
        SetInspector(node);
    }

    public void SetInspector(Node node)
    {
        transform.DestroyAllChildren();

        variableList = new();

        string[] linkEvent = { "True", "False" };
        variableList.Add(CreateVariable(VariableType.Dropdown, "LinkEvent", node, ScriptDataKey.LinkEvent, linkEvent));
        variableList.Add(CreateVariable(VariableType.Dropdown, "SkipMethod", node, ScriptDataKey.SkipMethod, Enum.GetNames(typeof(SkipMethod))));
        variableList.Add(CreateVariable(VariableType.InputField, "SkipDelay", node, ScriptDataKey.SkipDelay, null, null, InputField.ContentType.DecimalNumber));

        if (node.script.scriptType == ScriptType.Text)
        {
            variableList.Add(CreateVariable(VariableType.InputField, "Text", node, ScriptDataKey.Text));
            variableList.Add(CreateVariable(VariableType.InputField, "TextDuration", node, ScriptDataKey.TextDuration, null, null, InputField.ContentType.DecimalNumber));
            variableList.Add(CreateVariable(VariableType.InputField, "Audio 0", node, ScriptDataKey.Audio0));
            variableList.Add(CreateVariable(VariableType.InputField, "Audio 1", node, ScriptDataKey.Audio1));
        }
        else if (node.script.scriptType == ScriptType.Event)
        {
            EventType eventType = node.script.eventData.eventType;
            EventInfo eventInfo = EventInfo.GetEventInfo(eventType);

            variableList.Add(CreateVariable(VariableType.Dropdown, "Event", node, ScriptDataKey.EventType, Enum.GetNames(typeof(EventType))));
            variableList.Add(CreateVariable(VariableType.InputField, "EventDuration", node, ScriptDataKey.EventDuration, null, null, InputField.ContentType.DecimalNumber));

            if(eventInfo.canUseEventDuration == true)
            {
                variableList.Add(CreateVariable(VariableType.InputField, "DurationTurn", node, ScriptDataKey.DurationTurn, null, null, InputField.ContentType.IntegerNumber));
            }

            if(node.nodeType == Node.NodeType.BranchEnd)
            {
                return;
            }

            for (int i = 0; i < eventInfo.paramInfo.Count; ++i)
            {
                variableList.Add(CreateVariable(eventInfo.paramInfo[i], node));
            }
        }
    }

    public Variable CreateVariable(VariableType type, string name, Node targetNode, ScriptDataKey targetKey, string[] options = null, string explain = null, InputField.ContentType contentType = InputField.ContentType.Standard)
    {
        Variable var = Instantiate(variablePref).GetComponent<Variable>();

        var.Init(type);

        var.SetName(name);

        var.SetTarget(targetNode, targetKey);

        if(options != null)
        {
            var.SetDropdownOption(options);
        }

        var.SetValue(targetNode.script.GetVariableFromKey(targetKey));

        var.SetContentType(contentType);

        return var;
    }

    public Variable CreateVariable(EventParamInfo eventInfo, Node targetNode, InputField.ContentType contentType = InputField.ContentType.Standard)
    {
        Variable var = CreateVariable(eventInfo.varType, eventInfo.paramName, targetNode, eventInfo.targetKey, eventInfo.options, eventInfo.explain, contentType);

        var.eventParamInfo = eventInfo;

        return var;
    }

    public void ApplyInspector()
    {
        if (variableList == null)
        {
            return;
        }

        foreach (Variable var in variableList)
        {
            var.ApplyValue();
        }
    }

    public List<EventParamInfo> GetEventInfo(EventType eventType)
    {
        List<EventParamInfo> list = new();

        switch(eventType)
        {
            case EventType.CreateCharacter:
                list.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "캐릭터의 리소스 경로를 입력해주세요. (\"Assets/Resources/Image/\" 이후의 경로만 입력)"));
                string[] options = { "0", "1", "2", "3", "4" };
                list.Add(new(VariableType.Dropdown, "Position", ScriptDataKey.EventParam0, options, "캐릭터의 위치를 정해주세요."));
                break;
        }

        return list;
    }
}
