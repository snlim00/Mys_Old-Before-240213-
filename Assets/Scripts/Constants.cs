using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �빮�ڴ� ���̺��� ������ �� Ű�� ����ϴ� ������ �ǹ�
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
public enum SkipMethod
{
    Skipable, //��ŵ ����
    NoSkip, //��ŵ �Ұ���
    Auto, //���, �̺�Ʈ ���� ���� �ڵ����� ���� ��ũ��Ʈ�� �̵� (��ŵ �Ұ���)
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