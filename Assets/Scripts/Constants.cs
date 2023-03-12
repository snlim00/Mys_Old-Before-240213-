using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 대문자는 테이블을 가져올 때 키로 사용하는 값임을 의미
 */

public static class GameConstants
{
    public static bool isEditorMode = false;
}

public static class Constants //해당 프로젝트에서는 사용되지 않음. 221217
{
    public static int essentialKeyCount = 2;

    public static int idKey = 0;
    public static int nameKey = 1;
}

#region Script
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
    SetBackground,
    CreateCharacter,
    RemoveCharacter,
    RemoveAllCharacter,
    AddLovePoint,
    Goto,
    Branch,
}

public enum ScriptDataKey
{
    ScriptID,
    ScriptType,
    CharacterName,
    Text,
    TextDuration,
    SkipMethod,
    SkipDelay,
    LinkEvent,
    Audio0,
    Audio1,
    EventType,
    DurationTurn,
    EventDelay,
    EventDuration,
    LoopCount,
    LoopType,
    LoopDelay,
    EventParam0,
    EventParam1,
    EventParam2,
    EventParam3,
    EventParam4,
    EventParam5,
    EventParam6,
}
#endregion

public static class CharacterName
{
    public const string Jihyae = "Jihyae";
    public const string Yunha = "Yunha";
    public const string Seeun = "Seeun";
}