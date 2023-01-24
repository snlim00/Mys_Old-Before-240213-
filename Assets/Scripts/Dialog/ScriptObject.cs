using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public List<string> eventParam = new List<string>();

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
    public static readonly int UNVALID_ID = -1;
    public static readonly float DEFAULT_TEXT_DURATION = 0.1f;
    public static readonly SkipMethod DEFAULT_SKIP_METHOD = SkipMethod.Skipable;
    public static readonly float DEFAULT_SKIP_DELAY = 0.5f;
    public static readonly bool DEFAULT_LINK_EVENT = false;

    public int scriptID = UNVALID_ID;

    public string characterName = null;

    public string text = null;
    public float textDuration = DEFAULT_TEXT_DURATION;

    public string[] audio = new string[2];

    public SkipMethod skipMethod = DEFAULT_SKIP_METHOD;
    public float skipDelay = DEFAULT_SKIP_DELAY;

    public bool linkEvent = DEFAULT_LINK_EVENT;
    public EventData eventData;

    public bool isEvent
    {
        get { return eventData.eventType != EventType.None; }
    }

    public ScriptObject(string[] param)
    {
        eventData = new(this);

        scriptID = int.Parse(param[(int)KEY_SCRIPT_DATA.ScriptID]);

        characterName = param[(int)KEY_SCRIPT_DATA.CharacterName];

        text = param[(int)KEY_SCRIPT_DATA.Text];

        //TextDuration
        {
            float outValue;
            if (float.TryParse(param[(int)KEY_SCRIPT_DATA.TextDuration], out outValue))
            {
                textDuration = outValue;
            }
        }

        audio[0] = param[(int)KEY_SCRIPT_DATA.Audio0];
        audio[1] = param[(int)KEY_SCRIPT_DATA.Audio1];

        //SkipMethod
        {
            object outValue;

            if (Enum.TryParse(typeof(SkipMethod), param[(int)KEY_SCRIPT_DATA.SkipMethod], out outValue))
            {
                skipMethod = (SkipMethod)outValue;
            }
        }

        //SkipDelay
        {
            float outValue;

            if (float.TryParse(param[(int)KEY_SCRIPT_DATA.SkipDelay], out outValue))
            {
                skipDelay = outValue;
            }
        }

        //LinkEvent
        {
            bool outValue;

            if(bool.TryParse(param[(int)KEY_SCRIPT_DATA.LinkEvent], out outValue))
            {
                linkEvent = outValue;
            }
        }

        //eventDelay
        {
            float outValue;

            if (float.TryParse(param[(int)KEY_SCRIPT_DATA.EventDelay], out outValue))
            {
                eventData.eventDelay = outValue;
            }
        }

        //eventType
        {
            object outValue;

            if (Enum.TryParse(typeof(EventType), param[(int)KEY_SCRIPT_DATA.Event], out outValue))
            {
                eventData.eventType = (EventType)outValue;
            }
        }

        //durationTurn
        {
            int outValue;

            if (int.TryParse(param[(int)KEY_SCRIPT_DATA.DurationTurn], out outValue))
            {
                eventData.durationTurn = outValue;
            }
        }

        //eventDuration
        {
            float outValue;

            if (float.TryParse(param[(int)KEY_SCRIPT_DATA.EventDuration], out outValue))
            {
                eventData.eventDuration = outValue;
            }
        }

        //loopCount
        {
            int outValue;

            if (int.TryParse(param[(int)KEY_SCRIPT_DATA.LoopCount], out outValue))
            {
                eventData.loopCount = outValue;
            }
        }

        //loopType
        {
            object outValue;

            if(Enum.TryParse(typeof(LoopType), param[(int)KEY_SCRIPT_DATA.LoopType], out outValue))
            {
                eventData.loopType = (LoopType)outValue;
            }
        }
        
        //loopDelay
        {
            float outValue;

            if (float.TryParse(param[(int)KEY_SCRIPT_DATA.LoopDelay], out outValue))
            {
                eventData.loopDelay = outValue;
            }
        }

        //eventParam
        {
            int startValue = (int)KEY_SCRIPT_DATA.LoopDelay + 1;

            for (int i = startValue; i < param.Length; ++i)
            {
                eventData.eventParam.Add(param[i]);
            }
        }
    }
}