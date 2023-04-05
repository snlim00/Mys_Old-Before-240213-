using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

//얘를 정적 클래스로 만들지 말자.
public class ScriptManager
{
    public List<ScriptObject> scripts = new();

    public int currentIndex { get; private set; } = 0;
    public ScriptObject currentScript
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
    public void ReadAllScript()
    {
        string path = Application.dataPath + "/Data";
        var di = new System.IO.DirectoryInfo(path); // Assets/Data의 모든 파일 불러오기

        foreach(var file in di.GetFiles())
        {
            string name = file.Name;

            if(name.Contains(".CSV") && name.Contains(".meta") == false) //.CSV확장자의 파일 모두 불러오기 (.meta 파일은 제외)
            {
                ReadScript(name);
            }
        }
    }

    public void ReadScript(string path)
    {
        var script = CSVReader.ReadScript(path);

        scripts.AddRange(script);

        scripts.Sort(delegate (ScriptObject a, ScriptObject b) { return a.scriptID.CompareTo(b.scriptID); });
    }

    public void SetCurrentScript(ScriptObject script)
    {
        int index = scripts.IndexOf(script);

        if(index == -1)
        {
            (script.scriptID).LogWarning("스크립트를 찾을 수 없습니다. ScriptID");
            return;
        }

        currentIndex = index;
    }

    public void SetCurrentScript(int scriptID)
    {
        ScriptObject script = GetScriptFromID(scriptID);

        SetCurrentScript(script);
    }

    public ScriptObject Next()
    {
        ScriptObject nextScript = GetNextScript();

        SetCurrentScript(nextScript);

        return nextScript;
    }

    public ScriptObject GetNextScript()
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
            nextID.Log("다음 스크립트를 찾을 수 없습니다. ScriptID");
        }

        return nextScript;
    }

    public ScriptObject GetPrevScript()
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

    public ScriptObject GetPrevScriptFromID(int scriptID)
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

    public ScriptObject GetScriptFromID(int id)
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
