using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

//얘를 정적 클래스로 만들지 말자.
public static class ScriptManager
{
    public static List<ScriptObject> scripts = new();

    public static int currentIndex { get; private set; } = 0;
    public static ScriptObject currentScript
    {
        get
        {
            return scripts[currentIndex];
        }
    }

    /// <summary>
    /// Data/ScriptTable.CSV 파일을 읽어옴.<br/>
    /// 전체 게임 내에서 한 번만 호출.
    /// </summary>
    public static void ReadScript()
    {
        string path = Application.dataPath + "/Data";
        var di = new System.IO.DirectoryInfo(path); // Assets/Data의 모든 파일 불러오기

        foreach(var file in di.GetFiles())
        {
            string name = file.Name;

            if(name.Contains(".CSV") && name.Contains(".meta") == false) //.CSV확장자의 파일 모두 불러오기 (.meta 파일은 제외)
            {
                var script = CSVReader.ReadScript(name);

                scripts.AddRange(script);

                name.Log();
            }
        }
    }

    public static void SetCurrentScript(ScriptObject script)
    {
        int index = scripts.IndexOf(script);

        if(index == -1)
        {
            ("스크립트를 찾을 수 없습니다. ScriptID : " + script.scriptID).Log();
            return;
        }

        currentIndex = index;
    }

    public static void SetCurrentScript(int scriptID)
    {
        ScriptObject script = GetScriptFromID(scriptID);

        SetCurrentScript(script);
    }

    public static ScriptObject Next()
    {
        ScriptObject nextScript = GetNextScript();

        SetCurrentScript(nextScript);

        return nextScript;
    }

    public static ScriptObject GetNextScript()
    {
        int currID = currentScript.scriptID;
        int nextID = currentScript.scriptID + 1;

        if(IsSameScriptGroup(currID, nextID) == false)
        {
            ("다른 그룹의 스크립트에 접근했습니다. " + currID + ", " + nextID + " / " + GetGroupID(currID) + ", " + GetGroupID(nextID)).LogWarning();
            return null;
        }

        ScriptObject nextScript = GetScriptFromID(nextID);

        if(nextScript == null)
        {
            ("다음 스크립트를 찾을 수 없습니다. ScriptID : " + nextID).Log();
        }

        return nextScript;
    }

    public static ScriptObject GetPrevScript()
    {
        int currID = currentScript.scriptID;
        int prevID = currID - 1;
        
        if (IsSameScriptGroup(currID, prevID) == false)
        {
            ("다른 그룹의 스크립트에 접근했습니다. " + currID + ", " + prevID + " / " + GetGroupID(currID) + ", " + GetGroupID(prevID)).LogWarning();
            return null;
        }

        ScriptObject prevScript = GetScriptFromID(prevID);

        return prevScript;
    }

    public static ScriptObject GetPrevScriptFromID(int scriptID)
    {
        int currID = scriptID;
        int prevID = currID - 1;

        if (IsSameScriptGroup(currID, prevID) == false)
        {
            ("다른 그룹의 스크립트에 접근했습니다. " + currID + ", " + prevID + " / " + GetGroupID(currID) + ", " + GetGroupID(prevID)).LogWarning();
            return null;
        }

        ScriptObject prevScript = GetScriptFromID(prevID);

        return prevScript;
    }

    public static ScriptObject GetScriptFromID(int id)
    {
        foreach(var script in scripts)
        {
            if(script.scriptID == id)
            {
                return script;
            }
        }

        ("Script ID를 찾을 수 없습니다 : " + id).LogWarning();
        return null;
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

    public static bool IsSameScriptGroup(int id1, int id2)
    {
        return (GetGroupID(id1) == GetGroupID(id2));
    }
}
