using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveOption : MonoBehaviour
{
	private static string SavePath => Application.persistentDataPath + "/saves/";

    #region ������ ����
    public static void Save(OptionData saveData, string saveFileName)
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


    #region ������ �ε�
    public static OptionData Load(string saveFileName)
	{
		string saveFilePath = SavePath + saveFileName + ".json";
		Debug.Log("Saved Point : " + saveFilePath);

		if (!File.Exists(saveFilePath))
		{
			//	Debug.LogError("No such saveFile exists");
			return null;
		}

		string saveFile = File.ReadAllText(saveFilePath);
		OptionData saveData = JsonUtility.FromJson<OptionData>(saveFile);
		return saveData;
	}
    #endregion
}