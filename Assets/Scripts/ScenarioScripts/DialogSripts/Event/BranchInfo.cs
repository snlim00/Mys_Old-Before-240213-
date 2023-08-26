using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchBase
{
    public int scriptId;
}

public class BranchInfo : BranchBase
{
    public const int maxBranchAmount = 4; //Branch�� �ִ� ����
    public const int paramAmount = 2; //BranchInfo �ϳ��� ���� �Ķ���� ���� (���� �Ķ���� ����)
    public const int publicParamAmount = 2; //���� �Ķ���� ����

    public ConditionType conditionType;
    public string conditionName;
    public int conditionAmount;

    public static List<BranchInfo> CreateBranchInfo(ScriptObject script)
    {
        if (script.scriptType != ScriptType.Event || script.eventData.eventType != EventType.Branch)
        {
            return null;
        }

        EventData eventData = script.eventData;

        List<BranchInfo> branchInfos = new();

        for(int count = 0; count < maxBranchAmount; ++count)
        {
            bool allParamsFilled = true;
            for (int i = 0; i < paramAmount; ++i)
            {
                if (string.IsNullOrWhiteSpace(eventData.eventParam[publicParamAmount + (count * paramAmount) + i]))
                {
                    allParamsFilled = false;

                    if(i == 0)
                    {
                        return branchInfos;
                    }

                    break;
                }
            }

            if (allParamsFilled == true)
            {
                BranchInfo branchInfo = new();

                //���� �Ķ����
                if (Enum.TryParse(typeof(ConditionType), eventData.eventParam[0], out object outValue))
                {
                    branchInfo.conditionType = (ConditionType)outValue;
                }
                branchInfo.conditionName = eventData.eventParam[1];


                branchInfo.conditionAmount = int.Parse(eventData.eventParam[publicParamAmount + (count * paramAmount) + 0]);
                branchInfo.scriptId = int.Parse(eventData.eventParam[publicParamAmount + (count * paramAmount) + 1]);
            
                branchInfos.Add(branchInfo);
            }
            else
            {
                break;
            }
        }

        branchInfos[^1].conditionAmount = 0;

        return branchInfos;
    }
}
