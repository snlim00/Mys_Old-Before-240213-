using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RuntimeData
{
    public static bool isEditorMode = false;

    public static int saveFileNumber = 0;

    public static ScriptManager scriptMgr { get; set; }
}
