using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class EventParamInfo
{
    public string explain;

    public VariableType varType;
    public string paramName;
    public ScriptDataKey targetKey;
    public string[] options;
    public InputField.ContentType contentType = InputField.ContentType.Standard;

    public EventParamInfo(VariableType varType, string paramName, ScriptDataKey targetKey, string[] options = null, string explain = null, InputField.ContentType contentType = InputField.ContentType.Standard)
    {
        this.varType = varType;
        this.paramName = paramName;
        this.targetKey = targetKey;
        this.options = options;

        this.explain = explain;

        this.contentType = contentType;
    }
}

public class EventInfo
{
    public static Dictionary<EventType, EventInfo> infos = new();

    public string explain = null;

    public bool canUseDurationTurn = false;
    public bool canUseLoop = false;
    public bool canUseLinkEvent = true;

    public List<EventParamInfo> paramInfo = new();

    public static EventInfo GetEventInfo(EventType eventType)
    {
        if(infos.ContainsKey(eventType))
        {
            return infos[eventType];
        }
        else
        {
            return infos[EventType.None];
        }
    }

    public static void Init()
    {
        string[] characterNames = { CharacterName.Jihyae, CharacterName.Yunha, CharacterName.Seeun };

        //None
        {
            EventInfo info = new();

            info.canUseDurationTurn = false;
            info.canUseLoop = false;
            info.canUseLinkEvent = false;
            infos.Add(EventType.None, info);
        }

        //SetBackground
        {
            EventInfo info = new();

            info.canUseDurationTurn = true;
            info.canUseLoop = false;
            info.canUseLinkEvent = true;
            infos.Add(EventType.SetBackground, info);

            info.paramInfo.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "����� ���ҽ� ��θ� �Է����ּ���. (\"Assets/Resources/Image/Background/\" ������ ��θ� �Է�)\n* Ȯ���ڴ� �Է����� ����."));
        }

        //CreateObject
        {
            EventInfo info = new();

            info.canUseDurationTurn = true;
            info.canUseLoop = false;
            info.canUseLinkEvent = true;
            infos.Add(EventType.CreateObject, info);

            info.paramInfo.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "������Ʈ�� ���ҽ� ��θ� �Է����ּ���. (\"Assets/Resources/Image/Character/\" ������ ��θ� �Է�)\n* Ȯ���ڴ� �Է����� ����."));
            info.paramInfo.Add(new(VariableType.InputField, "Name", ScriptDataKey.EventParam1, null, "������Ʈ�� �̸��� �Է����ּ���."));
            string[] options = { "0", "1", "2", "3", "4" };
            info.paramInfo.Add(new(VariableType.Dropdown, "Position", ScriptDataKey.EventParam2, options, "������Ʈ�� ��ġ�� �����ּ���."));
        }

        //RemoveObject
        {
            EventInfo info = new();
            info.canUseDurationTurn = false;
            info.canUseLoop = false;
            info.canUseLinkEvent = true;
            infos.Add(EventType.RemoveObject, info);

            info.paramInfo.Add(new(VariableType.Object, "Object", ScriptDataKey.EventParam0, null, "������Ʈ�� �������ּ���."));
        }

        //RemoveAllObject
        {
            EventInfo info = new();
            info.canUseDurationTurn = false;
            info.canUseLoop = false;
            info.canUseLinkEvent = true;
            infos.Add(EventType.RemoveAllObject, info);
        }

        //Branch
        {
            EventInfo info = new();
            info.canUseDurationTurn = false;
            info.canUseLoop = false;
            info.canUseLinkEvent = false;
            infos.Add(EventType.Branch, info);

            info.paramInfo.Add(new(VariableType.Dropdown, "Character", ScriptDataKey.EventParam0, characterNames, "ĳ���͸� �������ּ���."));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 1", ScriptDataKey.EventParam1, null, "ȣ���� �䱸 ������ �����ּ���.", InputField.ContentType.IntegerNumber));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 2", ScriptDataKey.EventParam3, null, "ȣ���� �䱸 ������ �����ּ���.", InputField.ContentType.IntegerNumber));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 3", ScriptDataKey.EventParam5, null, "ȣ���� �䱸 ������ �����ּ���.", InputField.ContentType.IntegerNumber));
        }

        //AddLovePoint
        {
            EventInfo info = new();

            info.canUseDurationTurn = false;
            info.canUseLoop = false;
            info.canUseLinkEvent = true;
            infos.Add(EventType.AddLovePoint, info);

            info.paramInfo.Add(new(VariableType.Dropdown, "Character", ScriptDataKey.EventParam0, characterNames, "ĳ���͸� �������ּ���."));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount", ScriptDataKey.EventParam1, null, "ȣ������ ���� �����ּ���.", InputField.ContentType.IntegerNumber));
        }

        //Goto
        {     
            EventInfo info = new();
            info.canUseDurationTurn = false;
            info.canUseLoop = false;
            info.canUseLinkEvent = false;
            infos.Add(EventType.Goto, info);

            info.paramInfo.Add(new(VariableType.InputField, "Script", ScriptDataKey.EventParam0, null, "�̵��� ��ũ��Ʈ�� �������ּ���."));
        }
    }
}