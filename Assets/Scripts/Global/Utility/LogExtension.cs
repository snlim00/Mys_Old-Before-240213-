using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogExtension
{
    public static void Log(this object obj)
    {
        Debug.Log(obj); 
    }

    public static void LogError(this object obj)
    {
        Debug.LogError(obj);
    }
}
