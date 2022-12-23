using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
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
    ChangeCharacter,
    ChangeBackground,
}

//해당 클래스의 모든 멤버변수는 멤버함수에 의해서만 변경되어야 함.
[System.Serializable]
public class ScriptObject
{
    public static readonly int UNVALID_ID = -1;
    public static readonly float DEFAULT_TEXT_DURATION = 0.1f;
    public static readonly SkipMethod DEFAULT_SKIP_METHOD = SkipMethod.Skipable;
    public static readonly EventType DEFAULT_EVENT_TYPE = EventType.None;
    public static readonly float DEFAULT_SKIP_DURATION = 0.5f;
    public static readonly float DEFAULT_EVENT_DURATION = 0.5f;
    public static readonly int EVENT_PARAM_COUNT = 6;


    public int scriptID = UNVALID_ID;
    public string characterName = null;
    public string text = null;
    public float textDuration = DEFAULT_TEXT_DURATION;
    //public SkipMethod skipMethod = DEFAULT_SKIP_METHOD; //그냥 스트링으로 하는게 이후에 함수 호출할 때 백 배 편할 듯. Enum으로 관리할 이유가 딱히 없어보임. 221217
    //public EventType eventType = DEFAULT_EVENT_TYPE;
    public SkipMethod skipMethod = DEFAULT_SKIP_METHOD;
    public float skipDelay = DEFAULT_SKIP_DURATION;
    public string eventType = null;
    public float eventDuration = DEFAULT_EVENT_DURATION;
    public string[] eventParam = new string[EVENT_PARAM_COUNT];

    public void SetVariable(string key, string value)
    {
        if(value == "" || value == null)
        {
            return;
        }

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
                    text = value;
                    break;

                case KEY_SCRIPT_DATA.TextDuration:
                    float.TryParse(value, out textDuration);
                    break;

                case KEY_SCRIPT_DATA.SkipMethod:
                    skipMethod = (SkipMethod)Enum.Parse(typeof(SkipMethod), value);
                    break;

                case KEY_SCRIPT_DATA.SkipDelay:
                    float.TryParse(value, out skipDelay);
                    break;

                case KEY_SCRIPT_DATA.Event:
                    eventType = value;
                    break;

                case KEY_SCRIPT_DATA.EventDuration:
                    float.TryParse(value, out eventDuration);
                    break;

                case KEY_SCRIPT_DATA.EventParam0:
                    eventParam[0] = value;
                    break;

                case KEY_SCRIPT_DATA.EventParam1:
                    eventParam[1] = value;
                    break;

                case KEY_SCRIPT_DATA.EventParam2:
                    eventParam[2] = value;
                    break;

                case KEY_SCRIPT_DATA.EventParam3:
                    eventParam[3] = value;
                    break;

                case KEY_SCRIPT_DATA.EventParam4:
                    eventParam[4] = value;
                    break;
                
                case KEY_SCRIPT_DATA.EventParam5:
                    eventParam[5] = value;
                    break;
            }

            //this.Log();
        }
    }
}