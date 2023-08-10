using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    public const int tempSaveNumber = 1;

    public void Start()
    {
        LoadSaveFile();
    }

    private void LoadSaveFile()
    {
        SaveFile saveFile = SaveManager.Load(tempSaveNumber);

        if (saveFile == null)
        {
            saveFile = SaveFile.CreateNewSaveFile();
            
            SaveManager.Save(saveFile, tempSaveNumber);
        }

        GameData.saveFile = saveFile;
    }
}
