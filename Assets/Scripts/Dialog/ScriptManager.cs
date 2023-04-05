using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

//�긦 ���� Ŭ������ ������ ����.
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
    /// Data/ScriptTable.CSV ������ �о��.<br/>
    /// ��ü ���� ������ �� ���� ȣ��.
    /// </summary>
    public void ReadAllScript()
    {
        string path = Application.dataPath + "/Data";
        var di = new System.IO.DirectoryInfo(path); // Assets/Data�� ��� ���� �ҷ�����

        foreach(var file in di.GetFiles())
        {
            string name = file.Name;

            if(name.Contains(".CSV") && name.Contains(".meta") == false) //.CSVȮ������ ���� ��� �ҷ����� (.meta ������ ����)
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
            (script.scriptID).LogWarning("��ũ��Ʈ�� ã�� �� �����ϴ�. ScriptID");
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
            ("�ٸ� �׷��� ��ũ��Ʈ�� �����߽��ϴ�. " + currID + ", " + nextID + " / " + GetGroupID(currID) + ", " + GetGroupID(nextID)).LogWarning();
            return null;
        }

        ScriptObject nextScript = GetScriptFromID(nextID);

        if(nextScript == null)
        {
            nextID.Log("���� ��ũ��Ʈ�� ã�� �� �����ϴ�. ScriptID");
        }

        return nextScript;
    }

    public ScriptObject GetPrevScript()
    {
        int currID = currentScript.scriptID;
        int prevID = currID - 1;
        
        if (IsSameScriptGroup(currID, prevID) == false)
        {
            ("�ٸ� �׷��� ��ũ��Ʈ�� �����߽��ϴ�. " + currID + ", " + prevID + " / " + GetGroupID(currID) + ", " + GetGroupID(prevID)).LogWarning();
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
            ("�ٸ� �׷��� ��ũ��Ʈ�� �����߽��ϴ�. " + currID + ", " + prevID + " / " + GetGroupID(currID) + ", " + GetGroupID(prevID)).LogWarning();
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

        ("Script ID�� ã�� �� �����ϴ� : " + id).LogWarning();
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
