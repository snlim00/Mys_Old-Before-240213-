using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
	public Dictionary<string, int> chapterData;

	public Dictionary<string, int> status;

	public int money;

	public int date;

	public string selectedCharacter;

    public List<int> title;

	public static SaveFile CreateNewSaveFile()
	{
        var saveFile = new SaveFile();

        saveFile.chapterData = new()
        {
            { CharacterName.Jihyae, 0 },
            { CharacterName.Seeun, 0 },
            { CharacterName.Yunha, 0 }
        };

        saveFile.status = new()
        {
            { StatusName.STR, 0 },
            { StatusName.DEX, 0 },
            { StatusName.INT, 0 },
            { StatusName.LUK, 0 },
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