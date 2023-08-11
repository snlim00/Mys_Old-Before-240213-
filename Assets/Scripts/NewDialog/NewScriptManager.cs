using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewScriptManager
{
    public int scriptGroupId;

    public int chapter;

    public string title;

    public string explain;

    public Dictionary<string, int> requiredStat = new();

    public List<ScriptObject> scripts;
}
