using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveManager
{
	private static string SavePath => Application.persistentDataPath + "/saves/";

	#region 데이터 저장
	public static void Save(SaveFile saveData, int saveFileNumber)
	{
		
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}

		string saveJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);

		string saveFilePath = PathManager.CreateSaveFilePath(saveFileNumber);
        File.WriteAllText(saveFilePath, saveJson);
		Debug.Log("Save Success: " + saveFilePath);
	}
	#endregion


	#region 데이터 로드
	public static SaveFile Load(int saveFileNumber)
	{
		string saveFilePath = PathManager.CreateSaveFilePath(saveFileNumber);

        if (!File.Exists(saveFilePath))
		{
			SaveFile save = SaveFile.CreateNewSaveFile();

			Save(save, saveFileNumber);

            return save;
		}

		string saveFile = File.ReadAllText(saveFilePath);
		SaveFile saveData = JsonConvert.DeserializeObject<SaveFile>(saveFile);
		return saveData;
	}
    #endregion
}
