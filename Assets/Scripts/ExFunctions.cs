using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public static void LogWarning(this object obj)
    {
        Debug.LogWarning(obj);
    }

    public static void 로그(this object obj)
    {
        Debug.Log(obj);
    }

    public static void Log<T>(this T[] array)
    {
        string output = string.Join(", ", array);
        Debug.Log(output);
    }

    public static void Log(this ScriptObject obj)
    {
        //Debug.Log("{ " + obj.scriptID + ", " + obj.characterName+ ", " + obj.text + ", " + obj.textDuration + ", " + obj.skipMethod + ", " + obj.skipDelay + ", " + obj.eventType + ", " + obj.eventDuration + " }");
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

    public static void SetAlpha(this MaskableGraphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;

        graphic.color = color;
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        Transform[] childList = transform.GetComponentsInChildren<Transform>();

        foreach (var child in childList)
        {
            if (child != transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
