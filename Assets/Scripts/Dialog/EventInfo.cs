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

            info.paramInfo.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "배경의 리소스 경로를 입력해주세요. \n* Assets/Resources/Image/Background/ 이후의 경로만 입력\n* 확장자는 입력하지 않음."));
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

            info.paramInfo.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "오브젝트의 리소스 경로를 입력해주세요. \n* Assets/Resources/Image/Character/ 이후의 경로만 입력\n* 확장자는 입력하지 않음."));
            info.paramInfo.Add(new(VariableType.InputField, "Name", ScriptDataKey.EventParam1, null, "오브젝트의 이름을 입력해주세요."));
            string[] options = { "0", "1", "2", "3", "4" };
            info.paramInfo.Add(new(VariableType.Dropdown, "Position", ScriptDataKey.EventParam2, options, "오브젝트의 위치를 정해주세요."));
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

            info.paramInfo.Add(new(VariableType.Object, "Object", ScriptDataKey.EventParam0, null, "오브젝트를 선택해주세요."));
            string[] options = { "0", "1", "2", "3", "4" };
            info.paramInfo.Add(new(VariableType.Dropdown, "Position", ScriptDataKey.EventParam1, options, "오브젝트를 이동시킬 위치를 선택해주세요."));
            info.paramInfo.Add(new(VariableType.InputField, "Duration", ScriptDataKey.EventParam2, null, "이동의 소요 시간을 입력해주세요.", InputField.ContentType.DecimalNumber));
            info.paramInfo.Add(new(VariableType.Dropdown, "EaseType", ScriptDataKey.EventParam3, easeTypes, "이동의 보간 방식을 선택해주세요."));
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

            info.paramInfo.Add(new(VariableType.Object, "Object", ScriptDataKey.EventParam0, null, "오브젝트를 선택해주세요."));
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

            info.paramInfo.Add(new(VariableType.Dropdown, "Hide", ScriptDataKey.EventParam0, boolean, "텍스트 박스 숨김 여부를 선택해주세요."));
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

            info.paramInfo.Add(new(VariableType.Dropdown, "Character", ScriptDataKey.EventParam0, characterNames, "캐릭터를 선택해주세요."));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount", ScriptDataKey.EventParam1, null, "호감도의 양을 적어주세요.", InputField.ContentType.IntegerNumber));
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

            info.paramInfo.Add(new(VariableType.InputField, "Script", ScriptDataKey.EventParam0, null, "이동할 스크립트를 선택해주세요."));
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

            info.paramInfo.Add(new(VariableType.Dropdown, "Character", ScriptDataKey.EventParam0, characterNames, "캐릭터를 선택해주세요."));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 1", ScriptDataKey.EventParam1, null, "호감도 요구 조건을 적어주세요.", InputField.ContentType.IntegerNumber));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 2", ScriptDataKey.EventParam3, null, "호감도 요구 조건을 적어주세요.", InputField.ContentType.IntegerNumber));
            info.paramInfo.Add(new(VariableType.InputField, "Point Amount 3", ScriptDataKey.EventParam5, null, "호감도 요구 조건을 적어주세요.", InputField.ContentType.IntegerNumber));
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

            info.paramInfo.Add(new(VariableType.InputField, "Option 1", ScriptDataKey.EventParam1, null, "첫 번째 선택지를 입력해주세요."));
            info.paramInfo.Add(new(VariableType.InputField, "Option 2", ScriptDataKey.EventParam3, null, "두 번째 선택지를 입력해주세요."));
            info.paramInfo.Add(new(VariableType.InputField, "Option 3", ScriptDataKey.EventParam5, null, "세 번째 선택지를 입력해주세요."));
        }
    }
}