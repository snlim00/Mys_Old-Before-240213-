using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ScriptType
{
    Text,
    Event,
}

[System.Serializable]
public class EventData
{
    public static readonly float DEFAULT_EVENT_DELAY = 0;
    public static readonly EventType DEFAULT_EVENT_TYPE = EventType.None;
    public static readonly int DEFAULT_DURATION_TURN = 0;
    public static readonly float DEFAULT_EVENT_DURATION = 0.5f;
    public static readonly int DEFAULT_LOOP_COUNT = 0;
    public static readonly LoopType DEFAULT_LOOP_TYPE = LoopType.Restart;
    public static readonly float DEFAULT_LOOP_DELAY = 0;
    public static readonly int EVENT_PARAM_COUNT = 6;

    public float eventDelay = DEFAULT_EVENT_DELAY;
    public EventType eventType = DEFAULT_EVENT_TYPE;
    public int durationTurn = DEFAULT_DURATION_TURN;
    public float eventDuration = DEFAULT_EVENT_DURATION;
    public int loopCount = DEFAULT_LOOP_COUNT;
    public LoopType loopType = DEFAULT_LOOP_TYPE;
    public float loopDelay = DEFAULT_LOOP_DELAY;
    public List<string> eventParam = new();

    public ScriptObject script = null;

    public EventData(ScriptObject script)
    {
        this.script = script;
    }
}

public struct BranchInfo
{
    public EventType type;

    //type에 따라 둘 중 하나를 사용하게 됨
    public List<int> requiredValue; //EventType.Branch
    public List<string> choiceText; //EventType.Choice

    public List<int> targetID;
    
    public int Count
    {
        get
        {
            if(type == EventType.Branch)
            {
                return Mathf.Min(requiredValue.Count, targetID.Count);
            }
            else if(type == EventType.Choice)
            {
                return Mathf.Min(choiceText.Count, targetID.Count);
            }
            else
            {
                return 0;
            }
        }
    }

    public BranchInfo(EventType type, object value, List<int> targetID)
    {
        this.type = type;

        if(type == EventType.Branch)
        {
            this.requiredValue = (List<int>)value;
            this.choiceText = null;
        }
        else if(type == EventType.Choice)
        {
            this.requiredValue = null;
            this.choiceText = (List<string>)value;
        }
        else
        {
            this.requiredValue = null;
            this.choiceText = null;
        }

        this.targetID = targetID;
    }

    public void Log()
    {
        string message = "";
        if(type == EventType.Branch)
        {
            for(int i = 0; i < this.Count; ++i)
            {
                message += (requiredValue[i] + " | " + targetID[i] + "\n");
            }
        }
        else if(type == EventType.Choice)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                message += (choiceText[i] + " | " + targetID[i] + "\n");
            }
        }

        Debug.Log(message);
    }

    public static BranchInfo GetBranchInfo(in ScriptObject script)
    {
        EventData eventData = script.eventData;

        if (script.scriptType != ScriptType.Event)
        {
            return new BranchInfo(EventType.None, null, null);
        }

        if (eventData.eventType == EventType.Branch)
        {
            BranchInfo branchInfo = CreateBranchInfo(script);
            return branchInfo;

            //return CreateBranchInfo(script);
        }
        else if(eventData.eventType == EventType.Choice)
        {
            BranchInfo branchInfo = CreateChoiceInfo(script);
            return branchInfo;
            //return CreateChoiceInfo(script);
        }
        else
        {
            return new BranchInfo(EventType.None, null, null);
        }
    }

    private static BranchInfo CreateBranchInfo(in ScriptObject script)
    {
        EventData eventData = script.eventData;

        if (script.scriptType != ScriptType.Event || eventData.eventType != EventType.Branch)
        {
            script.Log();
            "ParseBranch - 해당 스크립트는 브랜치가 아닙니다.".LogError();
            return new BranchInfo(EventType.None, null, null);
        }

        List<int> requiredValue = new();
        List<int> targetScriptID = new();

        for (int i = 1; i < eventData.eventParam.Count; ++i)
        {
            if (i % 2 == 0)
            {
                if (int.TryParse(eventData.eventParam[i], out int value))
                {
                    targetScriptID.Add(value);
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (int.TryParse(eventData.eventParam[i], out int value))
                {
                    requiredValue.Add(value);
                }
                else
                {
                    break;
                }
            }
        }

        BranchInfo branchInfo = new(EventType.Branch, requiredValue, targetScriptID);

        return branchInfo;
    }

    private static BranchInfo CreateChoiceInfo(in ScriptObject script)
    {
        EventData eventData = script.eventData;

        if (eventData.eventType != EventType.Choice)
        {
            script.Log();
            "ParseBranch - 해당 스크립트는 브랜치가 아닙니다.".LogError();
            return new BranchInfo(EventType.None, null, null);
        }

        List<string> choiceText = new();
        List<int> targetScriptID = new();

        for (int i = 1; i < eventData.eventParam.Count; ++i)
        {
            if (i % 2 == 0)
            {
                if (int.TryParse(eventData.eventParam[i], out int value))
                {
                    targetScriptID.Add(value);
                }
                else
                {
                    break;
                }
            }
            else
            {
                string text = eventData.eventParam[i].ToString();

                if (text != "")
                {
                    choiceText.Add(text);
                }
                else
                {
                    break;
                }
            }
        }

        BranchInfo branchInfo = new(EventType.Choice, choiceText, targetScriptID);

        return branchInfo;
    }
}

//해당 클래스의 모든 멤버변수는 멤버함수에 의해서만 변경되어야 함.
[System.Serializable]
public class ScriptObject : ICloneable
{
    public static readonly int UNVALID_ID = -1;
    public static readonly ScriptType DEFAULT_SCRIPT_TYPE = ScriptType.Text;
    public static readonly float DEFAULT_TEXT_DURATION = 0.1f;
    public static readonly SkipMethod DEFAULT_SKIP_METHOD = SkipMethod.Skipable;
    public static readonly float DEFAULT_SKIP_DELAY = 0.5f;
    public static readonly bool DEFAULT_LINK_EVENT = false;

    
    public int scriptID = UNVALID_ID;

    public ScriptType scriptType = ScriptType.Text;

    public string characterName = null;

    public string text = null;
    public float textDuration = DEFAULT_TEXT_DURATION;

    public string[] audio = new string[2];

    public SkipMethod skipMethod = DEFAULT_SKIP_METHOD;
    public float skipDelay = DEFAULT_SKIP_DELAY;

    public bool linkEvent = DEFAULT_LINK_EVENT;
    public EventData eventData;

    public object Clone()
    {
        ScriptObject clone = new();

        foreach (ScriptDataKey key in Enum.GetValues(typeof(ScriptDataKey)))
        {
            string value = GetVariableFromKey(key);

            clone.SetVariable(key, value);
        }

        return clone;
    }

    public int GetBranchCount
    {
        get
        {
            return BranchInfo.GetBranchInfo(this).Count;
        }
    }

    public ScriptObject()
    {
        eventData = new(this);

        eventData.eventParam.Add(null);
        eventData.eventParam.Add(null);
        eventData.eventParam.Add(null);
        eventData.eventParam.Add(null);
        eventData.eventParam.Add(null);
        eventData.eventParam.Add(null);
        eventData.eventParam.Add(null);
    }

    public string GetVariableFromKey(ScriptDataKey key)
    {
        if(eventData.eventParam.Count < 7)
        {
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
        }

        switch (key)
        {
            case ScriptDataKey.ScriptID:
                return scriptID.ToString();

            case ScriptDataKey.ScriptType:
                return Enum.GetName(typeof(ScriptType), scriptType);

            case ScriptDataKey.CharacterName:
                return characterName;

            case ScriptDataKey.Text:
                return text;

            case ScriptDataKey.TextDuration:
                return textDuration.ToString();

            case ScriptDataKey.Audio0:
                return audio[0];

            case ScriptDataKey.Audio1:
                return audio[1];

            case ScriptDataKey.SkipMethod:
                return Enum.GetName(typeof(SkipMethod), skipMethod);

            case ScriptDataKey.SkipDelay:
                return skipDelay.ToString();

            case ScriptDataKey.LinkEvent:
                return linkEvent.ToString();

            case ScriptDataKey.EventType:
                return Enum.GetName(typeof(EventType), eventData.eventType);

            case ScriptDataKey.EventDelay:
                return eventData.eventDelay.ToString();

            case ScriptDataKey.DurationTurn:
                return eventData.durationTurn.ToString();

            case ScriptDataKey.EventDuration:
                return eventData.eventDuration.ToString();

            case ScriptDataKey.LoopCount:
                return eventData.loopCount.ToString();

            case ScriptDataKey.LoopType:
                return Enum.GetName(typeof(LoopType), eventData.loopType);

            case ScriptDataKey.LoopDelay:
                return eventData.loopDelay.ToString();

            case ScriptDataKey.EventParam0:
                try
                {
                    return eventData.eventParam[0];
                }
                catch
                {
                    return null;
                }

            case ScriptDataKey.EventParam1:
                try
                {
                    return eventData.eventParam[1];
                }
                catch
                {
                    return null;
                }

            case ScriptDataKey.EventParam2:
                return eventData.eventParam[2];

            case ScriptDataKey.EventParam3:
                return eventData.eventParam[3];

            case ScriptDataKey.EventParam4:
                return eventData.eventParam[4];

            case ScriptDataKey.EventParam5:
                return eventData.eventParam[5];

            case ScriptDataKey.EventParam6:
                return eventData.eventParam[6];

        }

        return null;
    }

    public void SetVariable(ScriptDataKey key, string value)
    {
        if (eventData.eventParam.Count < 7)
        {
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
            eventData.eventParam.Add("");
        }

        switch (key)
        {
            case ScriptDataKey.ScriptID:
                scriptID = int.Parse(value);
                break;

            case ScriptDataKey.ScriptType:
                {
                    if (Enum.TryParse(typeof(ScriptType), value, out object outValue))
                    {
                        scriptType = (ScriptType)outValue;
                    }
                }
                break;

            case ScriptDataKey.CharacterName:
                characterName = value;
                break;

            case ScriptDataKey.Text:
                text = value;
                break;

            case ScriptDataKey.TextDuration:
                {
                    if (float.TryParse(value, out float outValue))
                    {
                        textDuration = outValue;
                    }
                }
                break;

            case ScriptDataKey.Audio0:
                audio[0] = value;
                break;

            case ScriptDataKey.Audio1:
                audio[1] = value;
                break;

            case ScriptDataKey.SkipMethod:
                {
                    if (Enum.TryParse(typeof(SkipMethod), value, out object outValue))
                    {
                        skipMethod = (SkipMethod)outValue;
                    }
                }
                break;

            case ScriptDataKey.SkipDelay:
                {
                    if (float.TryParse(value, out float outValue))
                    {
                        skipDelay = outValue;
                    }
                }
                break;

            case ScriptDataKey.LinkEvent:
                {
                    if (bool.TryParse(value, out bool outValue))
                    {
                        linkEvent = outValue;
                    }
                }
                break;

            case ScriptDataKey.EventType:
                {
                    if (Enum.TryParse(typeof(EventType), value, out object outValue))
                    {
                        eventData.eventType = (EventType)outValue;
                    }
                }
                break;

            case ScriptDataKey.EventDelay:
                {
                    if (float.TryParse(value, out float outValue))
                    {
                        eventData.eventDelay = outValue;
                    }
                }
                break;

            case ScriptDataKey.DurationTurn:
                {
                    if (int.TryParse(value, out int outValue))
                    {
                        eventData.durationTurn = outValue;
                    }
                }
                break;

            case ScriptDataKey.EventDuration:
                {
                    if (float.TryParse(value, out float outValue))
                    {
                        eventData.eventDuration = outValue;
                    }
                }
                break;

            case ScriptDataKey.LoopCount:
                {
                    if (int.TryParse(value, out int outValue))
                    {
                        eventData.loopCount = outValue;
                    }
                }
                break;

            case ScriptDataKey.LoopType:
                {
                    if (Enum.TryParse(typeof(LoopType), value, out object outValue))
                    {
                        eventData.loopType = (LoopType)outValue;
                    }
                }
                break;

            case ScriptDataKey.LoopDelay:
                {
                    if (float.TryParse(value, out float outValue))
                    {
                        eventData.loopDelay = outValue;
                    }
                }
                break;

            case ScriptDataKey.EventParam0:
                eventData.eventParam[0] = value;
                break;

            case ScriptDataKey.EventParam1:
                eventData.eventParam[1] = value;
                break;

            case ScriptDataKey.EventParam2:
                eventData.eventParam[2] = value;
                break;

            case ScriptDataKey.EventParam3:
                eventData.eventParam[3] = value;
                break;

            case ScriptDataKey.EventParam4:
                eventData.eventParam[4] = value;
                break;

            case ScriptDataKey.EventParam5:
                eventData.eventParam[5] = value;
                break;

            case ScriptDataKey.EventParam6:
                eventData.eventParam[6] = value;
                break;

        }
    }

    public ScriptObject(string[] param)
    {
        eventData = new(this);

        for(int i = 0; i < param.Length; ++i)
        {
            SetVariable((ScriptDataKey)i, param[i]);
        }
    }
}