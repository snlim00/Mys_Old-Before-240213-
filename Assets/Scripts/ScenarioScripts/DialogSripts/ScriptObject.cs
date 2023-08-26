using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

//해당 클래스의 모든 멤버변수는 멤버함수에 의해서만 변경되어야 함.
[System.Serializable]
public class ScriptObject
{
    public static readonly int eventParamCount = 20;

    public static readonly int UNVALID_ID = -1;
    public static readonly ScriptType DEFAULT_SCRIPT_TYPE = ScriptType.Text;
    public static readonly float DEFAULT_TEXT_DURATION = 0.1f;
    public static readonly SkipMethod DEFAULT_SKIP_METHOD = SkipMethod.Skipable;
    public static readonly float DEFAULT_SKIP_DELAY = 0.5f;
    public static readonly bool DEFAULT_LINK_EVENT = false;


    public int scriptId = UNVALID_ID;

    public ScriptType scriptType = ScriptType.Text;

    public string characterName = null;

    public string text = null;
    public float textDuration = DEFAULT_TEXT_DURATION;

    public string[] audio = new string[2];

    public SkipMethod skipMethod = DEFAULT_SKIP_METHOD;
    public float skipDelay = DEFAULT_SKIP_DELAY;

    public bool linkEvent = DEFAULT_LINK_EVENT;
    public EventData eventData;

    public ScriptObject Clone()
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
            return BranchInfo_Old.GetBranchInfo(this).Count;
        }
    }

    public ScriptObject()
    {
        eventData = new(this);
    }

    public string GetVariableFromKey(ScriptDataKey key)
    {
        if (eventData.eventParam.Count < eventParamCount)
        {
            eventData.eventParam = new(new string[eventParamCount]);
        }

        switch (key)
        {
            case ScriptDataKey.ScriptId:
                return scriptId.ToString();

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

            default:
                if((int)key >= (int)ScriptDataKey.EventParam0)
                {
                    return eventData.eventParam[(int)key - (int)ScriptDataKey.EventParam0];
                }
                break;

        }

        return null;
    }

    public void SetVariable(ScriptDataKey key, string value)
    {
        if (eventData.eventParam.Count < eventParamCount)
        {
            eventData.eventParam = new(new string[eventParamCount]);
        }

        switch (key)
        {
            case ScriptDataKey.ScriptId:
                scriptId = int.Parse(value);
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


            default:
                if ((int)key >= (int)ScriptDataKey.EventParam0)
                {
                    eventData.eventParam[(int)key - (int)ScriptDataKey.EventParam0] = value;
                }
                break;

        }
    }

    public ScriptObject(string[] param)
    {
        eventData = new(this);

        for (int i = 0; i < param.Length; ++i)
        {
            SetVariable((ScriptDataKey)i, param[i]);
        }
    }

    public void Log()
    {
        string msg = string.Empty;

        foreach(ScriptDataKey key in Enum.GetValues(typeof(ScriptDataKey)))
        {
            msg += GetVariableFromKey(key) + " | ";
        }

        Debug.Log(msg);
    }
}