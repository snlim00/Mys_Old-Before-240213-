using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SkipMethod
{
    Skipable, //스킵 가능
    NoSkip, //스킵 불가능
    Auto, //대사, 이벤트 종료 이후 자동으로 다음 스크립트로 이동 (스킵 불가능)
    //WithNext, //해당 스크립트와 함께 다음 스크립트 즉시 시작. (다음 스크립트가 이벤트가 아니라면 무효 처리) //관련 처리는 SKipMethod가 아닌 별도의 파라미터로 처리하는 것이 좋을 듯. 221220
}

public enum EventType
{
    None,
    CreateCharacter,
    ChangeBackground,
}

public class EventData
{
    public static readonly EventType DEFAULT_EVENT_TYPE = EventType.None;
    public static readonly float DEFAULT_EVENT_DURATION = 0.5f;
    public static readonly int DEFAULT_LOOP_COUNT = 0;
    public static readonly float DEFAULT_LOOP_DELAY = 0;
    public static readonly int EVENT_PARAM_COUNT = 6;

    public EventType eventType = DEFAULT_EVENT_TYPE;
    public float eventDuration = DEFAULT_EVENT_DURATION;
    public int loopCount = DEFAULT_LOOP_COUNT;
    public float loopDelay = DEFAULT_LOOP_DELAY;
    public List<string> eventParam = new List<string>();

    public void SetVariable(string key, string value)
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
    public static readonly bool DEFAULT_LINK_TEXT = false;
    public static readonly float DEFAULT_TEXT_DURATION = 0.1f;
    public static readonly SkipMethod DEFAULT_SKIP_METHOD = SkipMethod.Skipable;
    public static readonly float DEFAULT_SKIP_DELAY = 0.5f;
    public static readonly bool DEFAULT_LINK_EVENT = false;

    public int scriptID = UNVALID_ID;
    public string characterName = null;
    public bool linkText = DEFAULT_LINK_TEXT;
    public string text = null;
    public float textDuration = DEFAULT_TEXT_DURATION;
    public SkipMethod skipMethod = DEFAULT_SKIP_METHOD;
    public float skipDelay = DEFAULT_SKIP_DELAY;
    public bool linkEvent = DEFAULT_LINK_EVENT;
    public EventData eventData = new EventData();

    public bool isEvent
    {
        get
        {
            if (eventData.eventType != EventType.None)
            {
                return true;
            }

            return false;
        }
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

                case KEY_SCRIPT_DATA.LinkText:
                    if (bool.TryParse(value, out linkText) == false)
                    {
                        linkText = DEFAULT_LINK_TEXT;
                    }
                    break;

                case KEY_SCRIPT_DATA.Text:
                    text = value;
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

                default:
                    eventData.SetVariable(key, value);
                    break;
            }
        }
        else
        {
            eventData.SetVariable(key, value);
        }
    }
}