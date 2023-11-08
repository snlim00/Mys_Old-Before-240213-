using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 내에서 사용되는 상수
public static class GameConstants
{
    public static bool isEditorMode = true;

    public const int firstScript = 9010;
}

//public static class Tag
//{
//    public static string Node = "Node";
//    public static string Variable = "Variable";
//    public static string VariableName = "VariableName";
//    public static string CharacterList = "CharacterList";
//}

//에디터 노드 타입
public enum NodeType
{
    Normal, //일반 노드
    Branch, //브랜치 노드
    BranchEnd, //브랜치의 끝 노드 (BranchEnd노드의 이벤트는 Goto지만, Branch의 마지막이라면 BranchEnd로 구분함)
    Goto, //Goto 노드
}

#region Script
//스크립트 파일의 섹션 이름들
public static class MysSection
{
    public static string chapter = "Chapter";
    public static string character = "Character";
    public static string title = "Title";
    public static string explain = "Explain";
    public static string requiredStat = "RequiredStat";
    public static string script = "Script";
}

//Branch 또는 Choice의 조건으로 사용될 수 있는 값의 목록
public enum ConditionType
{
    Stat, //능력치
    LovePoint, //호감도
}

//대화 씬에 생성되는 오브젝트의 태그
public enum DialogObjectTag
{
    Object,
    Jihyae,
    Yunha,
    Seeun
}

//ScriptObject의 타입
public enum ScriptType
{
    Text,
    Event,
}

//스킵 방식
public enum SkipMethod
{
    Skipable, //스킵 가능
    NoSkip, //스킵 불가능
    Auto, //대사, 이벤트 종료 이후 자동으로 다음 스크립트로 이동 (스킵 불가능)
}

//이벤트 종류
//이벤트를 추가하고 싶다면 여기에 이벤트명을 추가하고 EventManager에서 작업할 것.
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

//스크립트 파일의 키(헤더)
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

//캐릭터의 이름 상수와 캐릭터의 이름으로 가져올 수 있는 정보들을 모아둠
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

    public static Color GetCharacterTextColor(string name)
    {
        switch (name)
        {
            case Jihyae:
                return new(255 / 255f, 240 / 255f, 197 / 255f);

            case Yunha:
                return new(241 / 255f, 205 / 255f, 255 / 255f);

            case Seeun:
                return new(197 / 255f, 244 / 255f, 255 / 255f);
        }

        return Color.white;
    }

    public static Color GetCharacterTextShadowColor(string name)
    {
        switch (name)
        {
            case Jihyae:
                return new(255 / 255f, 240 / 255f, 197 / 255f, 0.5f);

            case Yunha:
                return new(241 / 255f, 205 / 255f, 255 / 255f, 0.5f);

            case Seeun:
                return new(197 / 255f, 244 / 255f, 255 / 255f, 0.5f);
        }

        return new(0, 0, 0, 0.5f);
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