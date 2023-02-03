using System.Collections;
using System.Collections.Generic;
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

    public string explain;
    public bool canUseEventDuration = false;

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
        //None
        {
            EventInfo info = new();
            info.canUseEventDuration = false;
            infos.Add(EventType.None, info);
        }

        //CreateCharacter
        {
            EventInfo info = new();

            info.canUseEventDuration = true;
            infos.Add(EventType.CreateCharacter, info);

            info.paramInfo.Add(new(VariableType.InputField, "Resource", ScriptDataKey.EventParam0, null, "캐릭터의 리소스 경로를 입력해주세요. (\"Assets/Resources/Image/\" 이후의 경로만 입력)"));
            string[] options = { "0", "1", "2", "3", "4" };
            info.paramInfo.Add(new(VariableType.Dropdown, "Position", ScriptDataKey.EventParam1, options, "캐릭터의 위치를 정해주세요."));
        }
    }
}