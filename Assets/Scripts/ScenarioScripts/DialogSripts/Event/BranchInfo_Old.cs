using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BranchInfo_Old
{
    public static BranchInfo_Old None => new BranchInfo_Old(EventType.None, null, null);

    public EventType type;

    //type에 따라 둘 중 하나를 사용하게 됨
    public List<int> requiredValue; //EventType.Branch
    public List<string> choiceText; //EventType.Choice

    public List<int> targetID;

    public int Count
    {
        get
        {
            if (type == EventType.Branch)
            {
                return Mathf.Min(requiredValue.Count, targetID.Count);
            }
            else if (type == EventType.Choice)
            {
                return Mathf.Min(choiceText.Count, targetID.Count);
            }
            else
            {
                return 0;
            }
        }
    }

    public BranchInfo_Old(EventType type, object value, List<int> targetID)
    {
        this.type = type;

        if (type == EventType.Branch)
        {
            this.requiredValue = (List<int>)value;
            this.choiceText = null;
        }
        else if (type == EventType.Choice)
        {
            this.requiredValue = null;
            this.choiceText = (List<string>)value;
        }
        else
        {
            this.requiredValue = null;
            this.choiceText = null;
        }

        this.targetID = targetID;
    }

    public void Log()
    {
        string message = "";
        if (type == EventType.Branch)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                message += (requiredValue[i] + " | " + targetID[i] + "\n");
            }
        }
        else if (type == EventType.Choice)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                message += (choiceText[i] + " | " + targetID[i] + "\n");
            }
        }

        Debug.Log(message);
    }

    public static BranchInfo_Old GetBranchInfo(in ScriptObject script)
    {
        EventData eventData = script.eventData;

        if (script.scriptType != ScriptType.Event)
        {
            return None;
        }

        if (eventData.eventType == EventType.Branch)
        {
            BranchInfo_Old branchInfo = CreateBranchInfo(script);
            return branchInfo;

            //return CreateBranchInfo(script);
        }
        else if (eventData.eventType == EventType.Choice)
        {
            BranchInfo_Old branchInfo = CreateChoiceInfo(script);
            return branchInfo;
            //return CreateChoiceInfo(script);
        }
        else
        {
            return None;
        }
    }

    private static BranchInfo_Old CreateBranchInfo(in ScriptObject script)
    {
        EventData eventData = script.eventData;

        if (script.scriptType != ScriptType.Event || eventData.eventType != EventType.Branch)
        {
            script.Log();
            "ParseBranch - 해당 스크립트는 브랜치가 아닙니다.".LogError();
            return None;
        }

        List<int> requiredValue = new();
        List<int> targetScriptID = new();

        for (int i = 1; i < eventData.eventParam.Count; ++i)
        {
            if (i % 2 == 0)
            {
                if (int.TryParse(eventData.eventParam[i], out int value))
                {
                    targetScriptID.Add(value);
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (int.TryParse(eventData.eventParam[i], out int value))
                {
                    requiredValue.Add(value);
                }
                else
                {
                    break;
                }
            }
        }

        BranchInfo_Old branchInfo = new(EventType.Branch, requiredValue, targetScriptID);

        return branchInfo;
    }

    private static BranchInfo_Old CreateChoiceInfo(in ScriptObject script)
    {
        EventData eventData = script.eventData;

        if (eventData.eventType != EventType.Choice)
        {
            script.Log();
            "ParseBranch - 해당 스크립트는 브랜치가 아닙니다.".LogError();
            return None;
        }

        List<string> choiceText = new();
        List<int> targetScriptID = new();

        for (int i = 1; i < eventData.eventParam.Count; ++i)
        {
            if (i % 2 == 0)
            {
                if (int.TryParse(eventData.eventParam[i], out int value))
                {
                    targetScriptID.Add(value);
                }
                else
                {
                    break;
                }
            }
            else
            {
                string text = eventData.eventParam[i].ToString();

                if (text != "")
                {
                    choiceText.Add(text);
                }
                else
                {
                    break;
                }
            }
        }

        BranchInfo_Old branchInfo = new(EventType.Choice, choiceText, targetScriptID);

        return branchInfo;
    }
}