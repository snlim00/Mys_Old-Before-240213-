using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 대문자는 테이블을 가져올 때 키로 사용하는 값을 의미
 */

public static class Constants //해당 프로젝트에서는 사용되지 않음. 221217
{
    public static int essentialKeyCount = 2;

    public static int idKey = 0;
    public static int nameKey = 1;
}

#region Script
public enum KEY_SCRIPT_DATA
{
    ScriptID,
    CharacterName,
    Text,
    TextDuration,
    SkipMethod,
    SkipDelay,
    WithEvent,
    Event,
    EventDuration,
    EventParam0,
    EventParam1,
    EventParam2,
    EventParam3,
    EventParam4,
    EventParam5,
}
#endregion