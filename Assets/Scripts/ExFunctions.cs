using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExFunctions
{
    public static void Log(this object obj)
    {
        Debug.Log(obj);
    }

    public static void ·Î±×(this object obj)
    {
        Debug.Log(obj);
    }

    public static void Log(this object[] obj)
    {
        string msg = "{ ";

        if(obj.Length > 1)
        {
            for(int i = 0; i < obj.Length - 1; ++i)
            {
                msg += obj[i] + ", ";
            }
            msg += obj[obj.Length - 2] + " }";
        }
        else
        {
            msg = "{ " + obj[0] + " }";
        }


        Debug.Log(msg);
    }

    public static void Log(this ScriptObject obj)
    {
        Debug.Log("{ " + obj.scriptID + ", " + obj.characterName+ ", " + obj.text + ", " + obj.textDuration + ", " + obj.skipMethod + ", " + obj.skipDuration + ", " + obj.eventType + ", " + obj.eventDuration + " }");
    }

    public static int TextLength(this string richText)
    {
        int len = 0;
        bool inTag = false;

        foreach (var ch in richText)
        {
            if (ch == '<')
            {
                inTag = true;
                continue;
            }
            else if (ch == '>')
            {
                inTag = false;
            }
            else if (!inTag && ch != ' ')
            {
                len++;
            }
        }

        return len;
    }
}
