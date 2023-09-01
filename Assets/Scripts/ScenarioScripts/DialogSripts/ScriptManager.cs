using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager
{
    public int scriptGroupId;

    public string character;

    public int chapter;

    public string title;

    public string explain;

    public Dictionary<string, int> requiredStat = new()
    {
        { StatInfo.STR, 0 },
        { StatInfo.DEX, 0 },
        { StatInfo.INT, 0 },
        { StatInfo.LUK, 0 },
    };

    public List<ScriptObject> scripts;


    public int currentScriptIndex { get; private set; } = 0;

    public ScriptObject currentScript
    {
        get
        {
            if (currentScriptIndex < scripts.Count)
            {
                return scripts[currentScriptIndex];
            }
            else
            {
                return null;
            }
        }
    }

    public ScriptObject nextScript
    {
        get
        {
            int nextIndex = currentScriptIndex + 1;

            if (nextIndex < scripts.Count)
            {
                return scripts[nextIndex];
            }
            else
            {
                return null;
            }
        }
    }

    public ScriptObject prevScript
    {
        get
        {
            int nextIndex = currentScriptIndex - 1;

            if (nextIndex >= 0)
            {
                return scripts[nextIndex];
            }
            else
            {
                return null;
            }
        }
    }

    public ScriptObject firstScript
    {
        get
        {
            return scripts[0];
        }
    }

    public ScriptObject Prev()
    {
        currentScriptIndex -= 1;

        return currentScript;
    }

    public ScriptObject Next()
    {
        currentScriptIndex += 1;

        return currentScript;
    }

    public ScriptObject GotoFirstScript()
    {
        currentScriptIndex = 0;

        return currentScript;
    }

    public ScriptObject GetScript(int scriptId)
    {
        ScriptObject script = scripts.Find(script => script.scriptId == scriptId);

        return script;
    }

    public ScriptObject SetCurrentScript(int scriptId)
    {
        ScriptObject script = GetScript(scriptId);

        return SetCurrentScript(script);
    }

    public ScriptObject SetCurrentScript(ScriptObject script)
    {
        int idx = scripts.IndexOf(script);

        currentScriptIndex = idx;

        return currentScript;
    }

    public static int GetScriptGroupId(int scriptId)
    {
        int groupID = (int)Mathf.Floor(scriptId / 10000f);

        return groupID;
    }

    public static int GetNextScenarioId(string characterName, int lastClearedChapter)
    {
        int nextScenario = GetFirstScenarioId(characterName);

        nextScenario += 10 * lastClearedChapter; //DEMO : 데모 버전에서는 브랜치 등 외부 요소가 없으므로 그냥 10을 더해서 사용.

        return nextScenario;
    }

    public static int GetFirstScenarioId(string characterName)
    {
        switch(characterName)
        {
            case CharacterInfo.Jihyae:
                return 1010;

            case CharacterInfo.Yunha:
                return 2010;

            case CharacterInfo.Seeun:
                return 3010;
        }

        return 0;
    }
}
