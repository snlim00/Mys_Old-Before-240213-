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

    public ScriptObject SetCurrentScript(int scriptId)
    {
        ScriptObject script = scripts.Find(script => script.scriptID == scriptId);

        return SetCurrentScript(script);
    }

    public ScriptObject SetCurrentScript(ScriptObject script)
    {
        int idx = scripts.IndexOf(script);

        currentScriptIndex = idx;

        return currentScript;
    }
}