using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public bool canUseEventDuration = true;
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
        string[] boolean = { true.ToString(), false.ToString() };
        string[] easeTypes = Enum.GetNames(typeof(Ease));
        string[] characterNames = { CharacterName.Jihyae, CharacterName.Yunha, CharacterName.Seeun };

        //None
        {
            EventInfo info = new()
            {
                canUseEventDuration = false,
                canUseDurationTurn = false,
                canUseLoop = false,
                canUseLinkEvent = false
            };
            infos.Add(EventType.None, info);
        }

        //SetBackground
        {
            EventInfo info = new()
            {
                canUseEventDuration = true,
                canUseDurationTurn = true,
                canUseLoop = false,
                canUseLinkEvent = true
            };
            infos.Add(EventType.SetBackground, info);

            info.paramInfo.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "����� ���ҽ� ��θ� �Է����ּ���. \n* Assets/Resources/Image/Background/ ������ ��θ� �Է�\n* Ȯ���ڴ� �Է����� ����."));
        }

        //CreateObject
        {
            EventInfo info = new()
            {
                canUseEventDuration = true,
                canUseDurationTurn = true,
                canUseLoop = false,
                canUseLinkEvent = true
            };
            infos.Add(EventType.CreateObject, info);

            info.paramInfo.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "������Ʈ�� ���ҽ� ��θ� �Է����ּ���. \n* Assets/Resources/Image/Character/ ������ ��θ� �Է�\n* Ȯ���ڴ� �Է����� ����."));
            info.paramInfo.Add(new(VariableType.InputField, "Name", ScriptDataKey.EventParam1, null, "������Ʈ�� �̸��� �Է����ּ���."));
            string[] options = { "0", "1", "2", "3", "4" };
            info.paramInfo.Add(new(VariableType.Dropdown, "Position", ScriptDataKey.EventParam2, options, "������Ʈ�� ��ġ�� �����ּ���."));
        }

        //MoveObject
        {
            EventInfo info = new()
            {
                canUseEventDuration = true,
                canUseDurationTurn = true,
                canUseLoop = false,
                canUseLinkEvent = true
            };
            infos.Add(EventType.MoveObject, info);

            info.paramInfo.Add(new(VariableType.Object, "Object", ScriptDataKey.EventParam0, null, "������Ʈ�� �������ּ���."));
            string[] options = { "0", "1", "2", "3", "4" };
            info.paramInfo.Add(new(VariableType.Dropdown, "Position", ScriptDataKey.EventParam1, options, "������Ʈ�� �̵���ų ��ġ�� �������ּ���."));
            info.paramInfo.Add(new(VariableType.InputField, "Duration", ScriptDataKey.EventParam2, null, "�̵��� �ҿ� �ð��� �Է����ּ���.", InputField.ContentType.DecimalNumber));
            info.paramInfo.Add(new(VariableType.Dropdown, "EaseType", ScriptDataKey.EventParam3, easeTypes, "�̵��� ���� ����� �������ּ���."));
        }

        //RemoveObject
        {
            EventInfo info = new()
            {
                canUseEventDuration = true,
                canUseDurationTurn = false,
                canUseLoop = false,
                canUseLinkEvent = true
            };
            infos.Add(EventType.RemoveObject, info);

            info.paramInfo.Add(new(VariableType.Object, "Object", ScriptDataKey.EventParam0, null, "������Ʈ�� �������ּ���."));
        }

        //RemoveAllObject
        {
            EventInfo info = new()
            {
                canUseEventDuration = false,
                canUseDurationTurn = false,
                canUseLoop = false,
                canUseLinkEvent = true
            };
            infos.Add(EventType.RemoveAllObject, info);
        }

        //HideTextBox
        {
            EventInfo info = new()
            {
                canUseEventDuration = true,
                canUseDurationTurn = true,
                canUseLoop = false,
                canUseLinkEvent = true
            };
            infos.Add(EventType.HideTextBox, info);

            info.paramInfo.Add(new(VariableType.Dropdown, "Hide", ScriptDataKey.EventParam0, boolean, "�ؽ�Ʈ �ڽ� ���� ���θ� �������ּ���."));
        }

        //AddLovePoint
        {
            EventInfo info = new()
            {
                canUseEventDuration = false,
                canUseDurationTurn = false,
                canUseLoop = false,
                canUseLinkEvent = true
            };
            infos.Add(EventType.AddLovePoint, info);

            info.paramInfo.Add(new(VariableType.Dropdown, "Character", ScriptDataKey.EventParam0, characterNames, "ĳ���͸� �������ּ���."));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount", ScriptDataKey.EventParam1, null, "ȣ������ ���� �����ּ���.", InputField.ContentType.IntegerNumber));
        }

        //Goto
        {
            EventInfo info = new()
            {
                canUseEventDuration = false,
                canUseDurationTurn = false,
                canUseLoop = false,
                canUseLinkEvent = false
            };
            infos.Add(EventType.Goto, info);

            info.paramInfo.Add(new(VariableType.InputField, "Script", ScriptDataKey.EventParam0, null, "�̵��� ��ũ��Ʈ�� �������ּ���."));
        }

        //Branch
        {
            EventInfo info = new()
            {
                canUseEventDuration = false,
                canUseDurationTurn = false,
                canUseLoop = false,
                canUseLinkEvent = false
            };
            infos.Add(EventType.Branch, info);

            info.paramInfo.Add(new(VariableType.Dropdown, "Character", ScriptDataKey.EventParam0, characterNames, "ĳ���͸� �������ּ���."));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 1", ScriptDataKey.EventParam1, null, "ȣ���� �䱸 ������ �����ּ���.", InputField.ContentType.IntegerNumber));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 2", ScriptDataKey.EventParam3, null, "ȣ���� �䱸 ������ �����ּ���.", InputField.ContentType.IntegerNumber));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 3", ScriptDataKey.EventParam5, null, "ȣ���� �䱸 ������ �����ּ���.", InputField.ContentType.IntegerNumber));
        }

        //Choice
        {
            EventInfo info = new()
            {
                canUseEventDuration = true,
                canUseDurationTurn = false,
                canUseLoop = false,
                canUseLinkEvent = false
            };
            infos.Add(EventType.Choice, info);

            info.paramInfo.Add(new(VariableType.InputField, "Option 1", ScriptDataKey.EventParam1, null, "ù ��° �������� �Է����ּ���."));
            info.paramInfo.Add(new(VariableType.InputField, "Option 2", ScriptDataKey.EventParam3, null, "�� ��° �������� �Է����ּ���."));
            info.paramInfo.Add(new(VariableType.InputField, "Option 3", ScriptDataKey.EventParam5, null, "�� ��° �������� �Է����ּ���."));
        }
    }
}