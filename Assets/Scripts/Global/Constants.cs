using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public enum NodeType
{
    Normal,
    Branch,
    BranchEnd,
    Goto,
}

#region Script
public static class MysSection
{
    public static string chapter = "Chapter";
    public static string character = "Character";
    public static string title = "Title";
    public static string explain = "Explain";
    public static string requiredStat = "RequiredStat";
    public static string script = "Script";
}

public enum ConditionType
{
    Stat,
    LovePoint,
}

public enum DialogObjectTag
{
    Object,
    Jihyae,
    Yunha,
    Seeun
}

public enum ScriptType
{
    Text,
    Event,
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
    PlayBGM,
    FadeIn,
    CreateObject,
    MoveObject,
    FlipObject,
    SetObjectAlpha,
    SetObjectImage,
    SetObjectScale,
    RemoveObject,
    RemoveAllObject,
    HideTextBox,
    ShowTitle,
    ShowCg,
    AddLovePoint,
    Goto,
    Branch,
    BranchEnd, //BranchEnd는 Goto와 동일한 이벤트임. 에디터에서 구분하기 위해 사용
    Choice,
    CloseScenario,
}

public enum ScriptDataKey
{
    ScriptId,
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
    EventParam7,
    EventParam8,
    EventParam9,
    EventParam10,
    EventParam11,
    EventParam12,
    EventParam13,
    EventParam14,
    EventParam15,
    EventParam16,
    EventParam17,
    EventParam18,
    EventParam19,
}
#endregion

public static class CharacterInfo
{
    public const string Jihyae = "Jihyae";
    public const string Yunha = "Yunha";
    public const string Seeun = "Seeun";
    public const string Public = "Public";

    public static string[] GetCharacterNames()
    {
        string[] arr =
        {
            Jihyae, Yunha, Seeun
        };

        return arr;
    }

    public static int GetHeadPosition(string name)
    {
        switch(name)
        {
            case Jihyae:
                return 30;

            case Yunha: 
                return 31;

            case Seeun: 
                return 32;
        }

        return 0;
    }

    public static int GetHeadPosition(DialogObjectTag tag)
    {
        return GetHeadPosition(tag.ToString());
    }
}

public static class StatInfo
{
    public const string STR = "Stat1";
    public const string DEX = "Stat2";
    public const string INT = "Stat3";
    public const string LUK = "Stat4";

    public static string[] GetStatNames()
    {
        string[] arr =
        {
            STR, DEX, INT, LUK
        };

        return arr;
    }

    public static string GetStatName(int index)
    {
        switch(index)
        {
            case 0:
                return STR;

            case 1:
                return DEX;

            case 2:
                return INT;

            case 3:
                return LUK;
        }

        return STR;
    }
}