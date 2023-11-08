using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� ������ ���Ǵ� ���
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

//������ ��� Ÿ��
public enum NodeType
{
    Normal, //�Ϲ� ���
    Branch, //�귣ġ ���
    BranchEnd, //�귣ġ�� �� ��� (BranchEnd����� �̺�Ʈ�� Goto����, Branch�� �������̶�� BranchEnd�� ������)
    Goto, //Goto ���
}

#region Script
//��ũ��Ʈ ������ ���� �̸���
public static class MysSection
{
    public static string chapter = "Chapter";
    public static string character = "Character";
    public static string title = "Title";
    public static string explain = "Explain";
    public static string requiredStat = "RequiredStat";
    public static string script = "Script";
}

//Branch �Ǵ� Choice�� �������� ���� �� �ִ� ���� ���
public enum ConditionType
{
    Stat, //�ɷ�ġ
    LovePoint, //ȣ����
}

//��ȭ ���� �����Ǵ� ������Ʈ�� �±�
public enum DialogObjectTag
{
    Object,
    Jihyae,
    Yunha,
    Seeun
}

//ScriptObject�� Ÿ��
public enum ScriptType
{
    Text,
    Event,
}

//��ŵ ���
public enum SkipMethod
{
    Skipable, //��ŵ ����
    NoSkip, //��ŵ �Ұ���
    Auto, //���, �̺�Ʈ ���� ���� �ڵ����� ���� ��ũ��Ʈ�� �̵� (��ŵ �Ұ���)
}

//�̺�Ʈ ����
//�̺�Ʈ�� �߰��ϰ� �ʹٸ� ���⿡ �̺�Ʈ���� �߰��ϰ� EventManager���� �۾��� ��.
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
    BranchEnd, //BranchEnd�� Goto�� ������ �̺�Ʈ��. �����Ϳ��� �����ϱ� ���� ���
    Choice,
    CloseScenario,
}

//��ũ��Ʈ ������ Ű(���)
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

//ĳ������ �̸� ����� ĳ������ �̸����� ������ �� �ִ� �������� ��Ƶ�
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