using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceInfo : BranchBase
{
    public const int maxChoiceAmount = 4; //Choice의 총 선택지 갯수
    public const int paramAmount = 5; //ChoiceInfo 하나에 들어가는 파라미터의 갯수

    public string text;
    
    public ConditionType conditionType;
    public string conditionName;
    public int conditionAmount;

    public static List<ChoiceInfo> CreateChoiceInfo(ScriptObject script)
    {
        if (script.scriptType != ScriptType.Event || script.eventData.eventType != EventType.Choice)
        {
            return null;
        }

        EventData eventData = script.eventData;

        List<ChoiceInfo> choiceInfos = new();

        for (int count = 0; count < maxChoiceAmount; ++count)
        {
            bool allParamsFilled = true;
            for (int i = 0; i < paramAmount; ++i)
            {
                if(string.IsNullOrWhiteSpace(eventData.eventParam[(count * paramAmount) + i]))
                {
                    allParamsFilled = false;
                    break;
                }
            }

            if(allParamsFilled == true)
            {
                ChoiceInfo choiceInfo = new();

                choiceInfo.text = eventData.eventParam[(count * paramAmount) + 0];

                if (Enum.TryParse(typeof(ConditionType), eventData.eventParam[(count * paramAmount) + 1], out object outValue))
                {
                    choiceInfo.conditionType = (ConditionType)outValue;
                }

                choiceInfo.conditionName = eventData.eventParam[(count * paramAmount) + 2];
                choiceInfo.conditionAmount = int.Parse(eventData.eventParam[(count * paramAmount) + 3]);

                choiceInfo.scriptId = int.Parse(eventData.eventParam[(count * paramAmount) + 4]);

                choiceInfos.Add(choiceInfo);
            }
            else
            {
                break;
            }
        }

        return choiceInfos;
    }
}
