using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO : 스크립트의 getter, setter에 예외 처리 해주기. (배열 길이 초과, 없는 scriptID, scriptID 앞자리 변경 등) 221223
public static class ScriptManager
{
    private static int currentIndex = 0;
    private static ScriptObject currentScript = null;
    public static List<ScriptObject> scripts = new List<ScriptObject>();

    public static void ReadScript()
    {
        ScriptManager.scripts = CSVReader.ReadScript("Data/ScriptTable.CSV");
    }

    public static void SetCurrentIndex(int idx)
    {
        currentIndex = idx;

        ScriptObject script = scripts[currentIndex];
    }

    public static int GetCurrentIndex()
    {
        return currentIndex;
    }

    public static ScriptObject GetCurrentScript()
    {
        ScriptObject script = scripts[currentIndex];

        return script;
    }

    public static ScriptObject GetNextScript()
    {
        return scripts[currentIndex + 1];
    }

    public static int Next()
    {
        currentIndex += 1; //스크립트 앞 ID가 바뀌면 넘어가지 않도록 할 것.

        SetCurrentIndex(currentIndex);

        return currentIndex;
    }


    public static int GetCurrentScriptIndex()
    {
        return currentIndex;
    }

    public static int GetCurrentScriptID()
    {
        return currentScript.scriptID;
    }

    public static void SetScriptFromID(int scriptID)
    {
        bool isFind = false;

        foreach(ScriptObject script in scripts)
        {
            if(script.scriptID == scriptID)
            {
                int index = scripts.IndexOf(script);

                SetCurrentIndex(index);

                isFind = true;
            }
        }

        if(isFind == false)
        {
            ("Script ID를 찾을 수 없습니다 : " + scriptID).Log();
        }
    }
}
