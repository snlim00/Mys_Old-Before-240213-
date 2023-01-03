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

    public static void LogError(this object obj)
    {
        Debug.LogError(obj);
    }

    public static void 로그(this object obj)
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
        Debug.Log("{ " + obj.scriptID + ", " + obj.characterName+ ", " + obj.text + ", " + obj.textDuration + ", " + obj.skipMethod + ", " + obj.skipDelay + ", " + obj.eventType + ", " + obj.eventDuration + " }");
    }

    //테스트를 위해 추가했던 함수. 사용하지 않음
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
