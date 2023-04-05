using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData : MonoBehaviour
{
	private static string SavePath => Application.persistentDataPath + "/saves/";

	#region 데이터 저장
	public static void Save(SaveFile saveData, string saveFileName)
	{
		
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}

		string saveJson = JsonUtility.ToJson(saveData);

		string saveFilePath = SavePath + saveFileName + ".json";
		File.WriteAllText(saveFilePath, saveJson);
		Debug.Log("Save Success: " + saveFilePath);
	}
	#endregion


	#region 데이터 로드
	public static SaveFile Load(string saveFileName)
	{
		string saveFilePath = SavePath + saveFileName + ".json";
		//Debug.Log(SavePath);

		if (!File.Exists(saveFilePath))
		{
			//Debug.LogError("No such saveFile exists");
			return null;
		}

		string saveFile = File.ReadAllText(saveFilePath);
		SaveFile saveData = JsonUtility.FromJson<SaveFile>(saveFile);
		return saveData;
	}
	#endregion

}
