using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ExFunctions
{
    private static string CreateMessage(object message, object info = null)
    {
        string msg = "";

        if(info != null)
        {
            msg += info + " : ";
        }
            
        msg += message;

        return msg;
    }

    public static void Log(this object obj, object info = null, bool withToastMessage = false)
    {
        string msg = CreateMessage(obj, info);

        Debug.Log(msg);

        if(withToastMessage == true)
        {
            ToastManager.Print(msg);
        }
    }

    public static void LogError(this object obj, object info = null, bool withToastMessage = false)
    {
        string msg = CreateMessage(obj, info);

        Debug.LogError(msg);

        if (withToastMessage == true)
        {
            ToastManager.Print("[Error] " + msg);
        }
    }

    public static void LogWarning(this object obj, object info = null, bool withToastMessage = false)
    {
        string msg = CreateMessage(obj, info);

        Debug.LogWarning(msg);

        if (withToastMessage == true)
        {
            ToastManager.Print("[ ! ] " + msg);
        }
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
        string message = "";

        foreach(ScriptDataKey key in Enum.GetValues(typeof(ScriptDataKey)))
        {
            var value = obj.GetVariableFromKey(key);

            message += value + " | ";
        }

        Debug.Log(message);
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

    public static void SetButtonText(this Button button, string text)
    {
        Text outValue = null;

        if (button == null) return;

        if(button?.transform.GetChild(0).TryGetComponent<Text>(out outValue) ?? false)
        {
            outValue.text = text;
        }
    }

    public static string GetButtonText(this Button button)
    {
        Text outValue;

        if (button.transform.GetChild(0).TryGetComponent<Text>(out outValue))
        {
            return outValue.text;
        }

        return null;
    }
}
