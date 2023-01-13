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

    public void SetVariable(string key, string value, ScriptObject owner)
    {
        object objValue;
        KEY_SCRIPT_DATA enumValue;
        if (Enum.TryParse(typeof(KEY_SCRIPT_DATA), key, out objValue))
        {
            enumValue = (KEY_SCRIPT_DATA)objValue;

            switch(enumValue)
            {
                case KEY_SCRIPT_DATA.Event:
                    object eventObjValue;

                    if (Enum.TryParse(typeof(EventType), value, out eventObjValue))
                    {
                        eventType = (EventType)eventObjValue;
                    }
                    break;

                case KEY_SCRIPT_DATA.DurationTurn:
                    if (int.TryParse(value, out durationTurn) == false)
                    {
                        durationTurn = DEFAULT_DURATION_TURN;
                    }
                    break;

                case KEY_SCRIPT_DATA.EventDelay:
                    if (float.TryParse(value, out eventDelay) == false)
                    {
                        eventDelay = DEFAULT_EVENT_DELAY;
                    }
                    break;

                case KEY_SCRIPT_DATA.EventDuration:
                    if (float.TryParse(value, out eventDuration) == false)
                    {
                        eventDuration = DEFAULT_EVENT_DURATION;
                    }
                    break;

                case KEY_SCRIPT_DATA.LoopCount:
                    if (int.TryParse(value, out loopCount) == false)
                    {
                        loopCount = DEFAULT_LOOP_COUNT;
                    }
                    break;

                case KEY_SCRIPT_DATA.LoopType:
                    object loopObjValue;

                    if (Enum.TryParse(typeof(LoopType), value, out loopObjValue))
                    {
                        loopType = (LoopType)loopObjValue;
                    }
                    break;

                case KEY_SCRIPT_DATA.LoopDelay:
                    if (float.TryParse(value, out loopDelay) == false)
                    {
                        loopDelay = DEFAULT_LOOP_DELAY;
                    }
                    break;
            }
        }
        else
        {
            eventParam.Add(value); //어떤 Enum키에도 해당하지 않으면 이벤트 파라미터로 230103
        }
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
    public EventData eventData = new EventData();

    public bool isEvent
    {
        get { return eventData.eventType != EventType.None; }
    }

    public void SetVariable(string key, string value)
    {
        object objValue;
        KEY_SCRIPT_DATA enumValue;
        if (Enum.TryParse(typeof(KEY_SCRIPT_DATA), key, out objValue))
        {
            enumValue = (KEY_SCRIPT_DATA)objValue;

            switch (enumValue)
            {
                case KEY_SCRIPT_DATA.ScriptID:
                    int.TryParse(value, out scriptID);
                    break;

                case KEY_SCRIPT_DATA.CharacterName:
                    characterName = value;
                    break;

                case KEY_SCRIPT_DATA.Text:
                    text = value.Replace("<br>", "\r\n");
                    break;

                case KEY_SCRIPT_DATA.TextDuration:
                    if(float.TryParse(value, out textDuration) == false)
                    {
                        textDuration = DEFAULT_TEXT_DURATION;
                    }

                    break;

                case KEY_SCRIPT_DATA.SkipMethod:
                    object skipObjValue;

                    if (Enum.TryParse(typeof(SkipMethod), value, out skipObjValue))
                    {
                        skipMethod = (SkipMethod)skipObjValue;
                    }

                    break;

                case KEY_SCRIPT_DATA.SkipDelay:
                    if(float.TryParse(value, out skipDelay) == false)
                    {
                        skipDelay = DEFAULT_SKIP_DELAY;
                    }
                    break;

                case KEY_SCRIPT_DATA.LinkEvent:
                    if(bool.TryParse(value, out linkEvent) == false)
                    {
                        linkEvent = DEFAULT_LINK_EVENT;
                    }

                    break;

                case KEY_SCRIPT_DATA.Audio0:
                    audio[0] = value;
                    break;
                
                case KEY_SCRIPT_DATA.Audio1:
                    audio[1] = value;
                    break;

                default:
                    eventData.SetVariable(key, value, this);
                    break;
            }
        }
        else
        {
            eventData.SetVariable(key, value, this);
        }
    }
}