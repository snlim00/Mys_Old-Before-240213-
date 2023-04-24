using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager
{
    public static string scriptPath{ get { return Application.dataPath + "/Data/"; } }

    public static string CreateScriptPath(int scriptGroupID)
    {
        return scriptPath + "ScriptTable" + scriptGroupID + ".CSV";
    }
}
