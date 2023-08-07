using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 대문자는 테이블을 가져올 때 키로 사용하는 값임을 의미
 */

public static class GameConstants
{
    public static bool isEditorMode = true;
}

public static class Tag
{
    public static string Node = "Node";
    public static string Variable = "Variable";
    public static string VariableName = "VariableName";
    public static string CharacterList = "CharacterList";
}

#region Script
public static class MysSection
{
    public static string title = "Title";
    public static string chapter = "Chapter";
    public static string branch = "Branch";
}

public enum SkipMethod
{
    Skipable, //스킵 가능
    NoSkip, //스킵 불가능
    Auto, //대사, 이벤트 종료 이후 자동으로 다음 스크립트로 이동 (스킵 불가능)
}

public enum EventType
{
    None,
    SetBackground,
    CreateObject,
    MoveObject,
    RemoveObject,
    RemoveAllObject,
    HideTextBox,
    AddLovePoint,
    Goto,
    Branch,
    Choice,
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

public static class StatusName
{
    public const string STR = "STR";
    public const string DEX = "DEX";
    public const string INT = "INT";
    public const string LUK = "LUK";
}