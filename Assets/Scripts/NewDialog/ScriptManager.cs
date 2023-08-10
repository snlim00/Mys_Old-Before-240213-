using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager
{
    public List<ScriptObject> scripts;

    public int scriptGroupID { get; private set; } = -1;

    public int currentScriptID { get; private set; } = -1;

    public ScriptObject currentScript
    {
        get
        {
            return GetScript(currentScriptID);
        }
    }

    public void ReadScript(int scriptGroupID)
    {
        this.scriptGroupID = scriptGroupID;

        scripts = CSVReader.CreateScriptList(scriptGroupID);

        currentScriptID = GetFirstScriptIDFromGroupID(scriptGroupID);
    }

    public void SetCurrentScript(int scriptID)
    {
        currentScriptID = scriptID;
    }

    public ScriptObject GetScript(int scriptID)
    {
        foreach(var script in scripts)
        {
            if(script.scriptID == scriptID) 
            { 
                return script;
            }
        }

        return null;
    }

    public ScriptObject GetNextScript()
    {
        return GetScript(currentScriptID + 1);
    }

    public ScriptObject Next()
    {
        var next = GetNextScript();

        if(next == null)
        {
            return null;
        }

        SetCurrentScript(next.scriptID);

        return next;
    }

    public static int GetGroupID(int id)
    {
        int groupID = (int)Mathf.Floor(id / 10000f);

        return groupID;
    }

    public static int GetFirstScriptIDFromGroupID(int id)
    {
        return (id * 10000) + 1;
    }
}
