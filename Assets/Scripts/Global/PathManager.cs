using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager
{
    public static string scriptPath{ get { return Application.dataPath + "/Data/"; } }

    [System.Obsolete]
    public static string _CreateScriptPath(int scriptGroupID)
    {
        return scriptPath + "ScriptTable" + scriptGroupID + ".mys";
    }

    public static string CreateScriptTextPath(int scriptGroupID)
    {
        return scriptPath + "TextScript" + scriptGroupID + ".csv";
    }

    public static string CreateScriptPath(int scriptGroupID)
    {
        return scriptPath + "ScriptTable" + scriptGroupID + ".mys";
    }


    private static string SavePath => Application.persistentDataPath + "/saves/";

    public static string CreateSaveFilePath(int saveFileNumber)
    {
        return SavePath + "SaveFile" + saveFileNumber + ".json";
    }


    public static string CreateImagePath(string path)
    {
        return "Images/" + path;
    }

    public static string CreateCgImagePath(string path)
    {
        return "Images/Cg/" + path;
    }

    public static string CreateBackgroundImagePath(string path)
    {
        return "Images/Background/" + path;
    }

    public static string CreateBGMPath(string path)
    {
        return "Audio/BGM/" + path;
    }
}
