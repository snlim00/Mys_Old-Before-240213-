using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class ProgressData
{
    public static int scriptID;

    public static int lovePoint_Jihyae { get; private set; }
    public static int lovePoint_Yunha { get; private set; }
    public static int lovePoint_Seeun { get; private set; }

    public static void AddLovePoint(in ScriptObject script, string target, int amount)
    {
        switch (target)
        {
            case CharacterName.Jihyae:
                ProgressData.lovePoint_Jihyae += amount;
                break;

            case CharacterName.Yunha:
                ProgressData.lovePoint_Yunha += amount;
                break;

            case CharacterName.Seeun:
                ProgressData.lovePoint_Seeun += amount;
                break;

            default:
                (target).LogWarning("ProgressData.AddLovePoint - 캐릭터를 찾을 수 없습니다" + script.scriptID);
                break;
        }
    }

    public static int GetLovePointFromCharacter(in ScriptObject script, string target)
    {
        switch (target)
        {
            case CharacterName.Jihyae:
                return ProgressData.lovePoint_Jihyae;

            case CharacterName.Yunha:
                return ProgressData.lovePoint_Yunha;

            case CharacterName.Seeun:
                return ProgressData.lovePoint_Seeun;

            default:
                target.LogWarning("ProgressData.GetLovePointFromCharacter - 캐릭터를 찾을 수 없습니다." + script.scriptID);
                return -1;
        }
    }
}
