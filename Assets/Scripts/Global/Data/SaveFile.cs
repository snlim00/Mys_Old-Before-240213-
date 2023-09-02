using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
	public Dictionary<string, int> chapterData;

    public Dictionary<string, int> lovePoint;

	public Dictionary<string, int> stat;

	public int money;

	public int date;

	public string selectedCharacter;

    public List<int> title;

    public void AddLovePoint(in ScriptObject script, string target, int amount)
    {
        lovePoint[target] += amount;
    }

    public int GetConditionData(ChoiceInfo choiceInfo)
    {
        switch (choiceInfo.conditionType)
        {
            case ConditionType.Stat:
                return GameData.saveFile.stat[choiceInfo.conditionName];

            case ConditionType.LovePoint:
                return GameData.saveFile.lovePoint[choiceInfo.conditionName];
        }

        return -1;
    }

    public int GetConditionData(BranchInfo branchInfo)
    {
        switch (branchInfo.conditionType)
        {
            case ConditionType.Stat:
                return GameData.saveFile.stat[branchInfo.conditionName];

            case ConditionType.LovePoint:
                return GameData.saveFile.lovePoint[branchInfo.conditionName];
        }

        return -1;
    }

	public static SaveFile CreateNewSaveFile()
	{
        var saveFile = new SaveFile();

        saveFile.chapterData = new()
        {
            { CharacterInfo.Jihyae, 0 },
            { CharacterInfo.Seeun, 0 },
            { CharacterInfo.Yunha, 0 },
            { CharacterInfo.Public, 0 }
        };

        saveFile.lovePoint = new()
        {
            { CharacterInfo.Jihyae, 0 },
            { CharacterInfo.Seeun, 0 },
            { CharacterInfo.Yunha, 0 }
        };

        saveFile.stat = new()
        {
            { StatInfo.STR, 0 },
            { StatInfo.DEX, 0 },
            { StatInfo.INT, 0 },
            { StatInfo.LUK, 0 },
        };

        saveFile.money = 0;

        saveFile.date = 0;

        saveFile.selectedCharacter = string.Empty;

        saveFile.title = new()
        {
            90001
        };

        return saveFile;
    }
}