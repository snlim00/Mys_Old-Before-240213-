using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ParamInfo
{
    public VariableType varType;
    public ScriptDataKey targetKey;
    public string paramName;
    public string defaultValue;
    public string explain;
    public InputField.ContentType contentType = InputField.ContentType.Standard;
    public string[] options;

    public ParamInfo(VariableType varType, ScriptDataKey targetKey, string paramName, string defaultValue, string explain = null, InputField.ContentType contentType = InputField.ContentType.Standard, string[] options = null)
    {
        this.varType = varType;
        this.targetKey = targetKey;
        this.paramName = paramName;
        this.defaultValue = defaultValue;
        this.explain = explain;
        this.contentType = contentType;
        this.options = options;
    }
}

public class ScriptInfo
{
    public static Dictionary<EventType, ScriptInfo> eventInfos = new();

    public static Dictionary<ScriptDataKey, ParamInfo> scriptParamInfos = new();

    public string explain = null;

    public List<ScriptDataKey> excludedKeys = new();

    public List<ParamInfo> paramInfo = new();

    public ParamInfo GetParamInfo(ScriptDataKey key)
    {
        foreach(var paramInfo in this.paramInfo)
        {
            if (paramInfo.targetKey == key)
            {
                return paramInfo;
            }
        }

        return null;
    }

    public static void Init()
    {
        string[] characterName = CharacterInfo.GetCharacterNames();

        //Script
        {
            //ScriptParamInfo_LinkEvent
            {
                ParamInfo info = new ParamInfo(VariableType.Toggle, ScriptDataKey.LinkEvent, "Link Event", ScriptObject.DEFAULT_LINK_EVENT.ToString());
                scriptParamInfos.Add(ScriptDataKey.LinkEvent, info);
            }

            //ScriptParamInfo_SkipMethod
            {
                ParamInfo info = new ParamInfo(VariableType.Dropdown, ScriptDataKey.SkipMethod, "Skip Method", ScriptObject.DEFAULT_SKIP_METHOD.ToString(), options: Enum.GetNames(typeof(SkipMethod)));
                scriptParamInfos.Add(ScriptDataKey.SkipMethod, info);
            }

            //ScriptParamInfo_SkipDelay
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.SkipDelay, "Skip Delay", ScriptObject.DEFAULT_SKIP_DELAY.ToString(), contentType: InputField.ContentType.DecimalNumber);
                scriptParamInfos.Add(ScriptDataKey.SkipDelay, info);
            }

            //ScriptParamInfo_CharacterName
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.CharacterName, "Character Name", string.Empty);
                scriptParamInfos.Add(ScriptDataKey.CharacterName, info);
            }

            //ScriptParamInfo_TextDuration
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.TextDuration, "Text Duration", ScriptObject.DEFAULT_TEXT_DURATION.ToString(), contentType: InputField.ContentType.DecimalNumber);
                scriptParamInfos.Add(ScriptDataKey.TextDuration, info);
            }

            //ScriptParamInfo_Audio0
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.Audio0, "Audio 0", string.Empty);
                scriptParamInfos.Add(ScriptDataKey.Audio0, info);
            }

            //ScriptParamInfo_Audio1
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.Audio1, "Audio 0", string.Empty);
                scriptParamInfos.Add(ScriptDataKey.Audio1, info);
            }

            //ScriptParamInfo_Text
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.Text, "Text", string.Empty);
                scriptParamInfos.Add(ScriptDataKey.Text, info);
            }

            //ScriptParamInfo_EventType
            {
                ParamInfo info = new ParamInfo(VariableType.Dropdown, ScriptDataKey.EventType, "Event", EventData.DEFAULT_EVENT_TYPE.ToString(), options: Enum.GetNames(typeof(EventType)));
                scriptParamInfos.Add(ScriptDataKey.EventType, info);
            }

            //ScriptParamInfo_EventDelay
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.EventDelay, "Event Delay", EventData.DEFAULT_EVENT_DELAY.ToString(), contentType: InputField.ContentType.DecimalNumber);
                scriptParamInfos.Add(ScriptDataKey.EventDelay, info);
            }

            //ScriptParamInfo_EventDuration
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.EventDuration, "Event Duration", EventData.DEFAULT_EVENT_DURATION.ToString(), contentType: InputField.ContentType.DecimalNumber);
                scriptParamInfos.Add(ScriptDataKey.EventDuration, info);
            }

            //ScriptParamInfo_DurationTurn
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.DurationTurn, "Duration Turn", EventData.DEFAULT_DURATION_TURN.ToString(), contentType: InputField.ContentType.IntegerNumber);
                scriptParamInfos.Add(ScriptDataKey.DurationTurn, info);
            }

            //ScriptParamInfo_LoopCount
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.LoopCount, "Loop Count", EventData.DEFAULT_LOOP_COUNT.ToString(), contentType: InputField.ContentType.IntegerNumber);
                scriptParamInfos.Add(ScriptDataKey.LoopCount, info);
            }

            //ScriptParamInfo_LoopType
            {
                ParamInfo info = new ParamInfo(VariableType.Dropdown, ScriptDataKey.LoopType, "Loop Type", EventData.DEFAULT_LOOP_TYPE.ToString(), options: Enum.GetNames(typeof(LoopType)));
                scriptParamInfos.Add(ScriptDataKey.LoopType, info);
            }

            //ScriptParamInfo_LoopDelay
            {
                ParamInfo info = new ParamInfo(VariableType.InputField, ScriptDataKey.LoopDelay, "Loop Delay", EventData.DEFAULT_LOOP_DELAY.ToString(), contentType: InputField.ContentType.DecimalNumber);
                scriptParamInfos.Add(ScriptDataKey.LoopDelay, info);
            }
        }

        //Event
        {
            //EventInfo_None
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LinkEvent,
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.None, info);
            }

            //EventInfo_SetBackground
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.SetBackground, info);

                string explain0 = "배경의 리소스 경로를 입력해주세요. \n* Assets/Resources/Images/Background 이후의 경로만 입력 \n* 확장자는 입력하지 않음";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam0, "Resource", string.Empty, explain: explain0));
            }

            //EventInfo_PlayBGM
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.PlayBGM, info);

                string explain0 = "BGM의 리소스 경로를 입력해주세요. \n* Assets/Resources/Audio/BGM/ 이후의 경로만 입력 \n* 확장자는 입력하지 않음";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam0, "Resource", string.Empty, explain: explain0));
            }


            //EventInfo_FadeIn
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.FadeIn, info);

                string explain0 = "FadeIn을 진행할 지에 대한 여부를 선택해주세요. \n* 미선택 시 기존 FadeIn된 화면이 FadeOut 됩니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam0, "Do Show", true.ToString(), explain: explain0));
                string explain1 = "FadeIn된 화면이 자동으로 FadeOut하게 할 지에 대한 여부를 선택해주세요. \n* 페이드 인/아웃이 자동으로 연계됩니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam1, "Auto Fade Out", false.ToString(), explain: explain1));
                string explain2 = "FadeIn된 경우 FadeIn을 지속할 시간을 입력해주세요. \n* Auto Fade Out이 활성화되지 않은 경우, 의미 없는 값입니다.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam2, "Duration", 3.ToString(), explain: explain2, contentType: InputField.ContentType.DecimalNumber));
            }

            //EventInfo_CreateObject
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.CreateObject, info);

                string explain0 = "오브젝트의 이름을 정해주세요. \n* 구분을 위해 에디터에서만 사용됨";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam0, "Object Name", string.Empty, explain: explain0));
                string explain1 = "오브젝트의 리소스 경로를 입력해주세요. \n* Assets/Resources/Images/ 이후의 경로만 입력 \n* 확장자는 입력하지 않음";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam1, "Resource", string.Empty, explain: explain1));
                string explain2 = "오브젝트의 태그를 선택해주세요.";
                string[] option = Enum.GetNames(typeof(DialogObjectTag));
                info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam2, "Object Tag", DialogObjectTag.Object.ToString(), explain: explain2, options: option));
                string explain3 = "오브젝트의 위치를 입력해주세요.\n가장 왼쪽이 1, 가장 오른쪽이 5입니다. \n* 범위 밖의 숫자나 소수도 입력이 가능합니다.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam3, "Position", 3.ToString(), explain: explain3, contentType: InputField.ContentType.DecimalNumber));
                string explain4 = "오브젝트 머리의 위치를 입력해주세요.\n* 태그가 존재하는 캐릭터라면 입력하지 않아도 됩니다.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam4, "Head Position", 0.ToString(), explain: explain4, contentType: InputField.ContentType.DecimalNumber));
            }

            //EventInfo_MoveObject
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                    },
                };
                eventInfos.Add(EventType.MoveObject, info);

                string explain0 = "이동시킬 오브젝트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Object, ScriptDataKey.EventParam0, "Object", string.Empty, explain: explain0));
                string explain1 = "오브젝트의 위치를 입력해주세요.\n가장 왼쪽이 1, 가장 오른쪽이 5입니다. \n* 범위 밖의 숫자나 소수도 입력이 가능합니다.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam1, "Position", 3.ToString(), explain: explain1, contentType: InputField.ContentType.DecimalNumber));
            }

            //EventInfo_FlipObject
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.FlipObject, info);

                string explain0 = "좌우 반전시킬 오브젝트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Object, ScriptDataKey.EventParam0, "Object", string.Empty, explain: explain0));
            }

            //EventInfo_SetObjectAlpha
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.SetObjectAlpha, info);

                string explain0 = "투명도를 변경할 오브젝트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Object, ScriptDataKey.EventParam0, "Object", string.Empty, explain: explain0));
                string explain1 = "투명도를 입력해주세요. (0 ~ 1)";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam1, "Alpha", 1.ToString(), explain: explain1, contentType: InputField.ContentType.DecimalNumber));
            }

            //EventInfo_SetObjectImage
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.EventDuration,
                    },
                };
                eventInfos.Add(EventType.SetObjectImage, info);

                string explain0 = "이미지를 변경할 오브젝트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Object, ScriptDataKey.EventParam0, "Object", string.Empty, explain: explain0));
                string explain1 = "오브젝트의 리소스 경로를 입력해주세요. \n* Assets/Resources/Images/ 이후의 경로만 입력 \n* 확장자는 입력하지 않음";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam1, "Resource", string.Empty, explain: explain1));
            }


            //EventInfo_SetObjectScale
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.SetObjectScale, info);

                string explain0 = "크기를 변경할 오브젝트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Object, ScriptDataKey.EventParam0, "Object", string.Empty, explain: explain0));
                string explain1 = "변경할 오브젝트의 크기를 입력해주세요.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam1, "Scale", 1.ToString(), explain: explain1, contentType: InputField.ContentType.DecimalNumber));
                string explain2 = "위를 기준으로 위치를 조정할 지에 대한 여부를 선택해주세요.\n* 선택 시 위를 기준으로 오브젝트의 위치가 조정됩니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam2, "Fitting Top", true.ToString(), explain: explain2));
            }


            //EventInfo_RemoveObject
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.RemoveObject, info);

                string explain0 = "제거할 오브젝트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Object, ScriptDataKey.EventParam0, "Object", string.Empty, explain: explain0));
            }


            //EventInfo_RemoveAllObject
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LinkEvent,
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.RemoveAllObject, info);
            }

            //EventInfo_HideTextBox
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.HideTextBox, info);

                string explain0 = "텍스트 박스를 숨길 지에 대한 여부를 선택해주세요. \n* 선택 시 텍스트 박스가 숨겨집니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam0, "Do Hide", true.ToString(), explain: explain0));
            }

            //EventInfo_ShowTitle
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.ShowTitle, info);

                string explain0 = "타이틀을 보이게 할 지에 대한 여부를 선택해주세요. \n* 선택 시 타이틀이 보여집니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam0, "Do Show", true.ToString(), explain: explain0));
                string explain1 = "타이틀이 자동으로 사라지게 할 지에 대한 여부를 선택해주세요. \n* 선택 시 타이틀이 자동으로 사라집니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam1, "Auto Hide", false.ToString(), explain: explain1));
                string explain2 = "Auto Hide가 활성화 된 경우, 타이틀이 보여지는 시간을 선택해주세요. \n* Auto Hide가 활성화되지 않은 경우, 의미 없는 값입니다.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam2, "Duration", 3.ToString(), explain: explain2, contentType: InputField.ContentType.DecimalNumber));
                string explain3 = "메인 타이틀에 보여질 텍스트를 입력해주세요.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam3, "Title", string.Empty, explain: explain3));
                string explain4 = "서브 타이틀에 보여질 텍스트를 입력해주세요.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam4, "Subtitle", string.Empty, explain: explain4));
            }

            //EventInfo_ShowCg
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {

                    },
                };
                eventInfos.Add(EventType.ShowCg, info);

                string explain0 = "CG를 보이게 할 지에 대한 여부를 선택해주세요. \n* 선택 시 타이틀이 보여집니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam0, "Do Show", true.ToString(), explain: explain0));
                string explain1 = "CG가 자동으로 사라지게 할 지에 대한 여부를 선택해주세요. \n* 선택 시 타이틀이 자동으로 사라집니다.";
                info.paramInfo.Add(new(VariableType.Toggle, ScriptDataKey.EventParam1, "Auto Hide", false.ToString(), explain: explain1));
                string explain2 = "Auto Hide가 활성화 된 경우, CG가 보여지는 시간을 선택해주세요. \n* Auto Hide가 활성화되지 않은 경우, 의미 없는 값입니다.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam2, "Duration", 3.ToString(), explain: explain2, contentType: InputField.ContentType.DecimalNumber));
                string explain3 = "Cg 이미지의 리소스 경로를 입력해주세요. \n* Assets/Resources/Images/Cg/ 이후의 경로만 입력 \n* 확장자는 입력하지 않음";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam3, "Resource", string.Empty, explain: explain3));
            }

            //EventInfo_AddLovePoint
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.AddLovePoint, info);

                string explain0 = "호감도를 변경할 대상을 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam0, "Character", CharacterInfo.Seeun, explain: explain0, options: characterName));
            }

            //EventInfo_Goto
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LinkEvent,
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.Goto, info);

                string explain0 = "이동할 스크립트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam0, "Script", string.Empty, explain: explain0));
            }

            //EventInfo_BranchEnd
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LinkEvent,
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    },
                };
                eventInfos.Add(EventType.BranchEnd, info);

                string explain0 = "이동할 스크립트를 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam0, "Script", ScriptVariable.autoTracking, explain: explain0));
            }

            //EventInfo_Branch
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LinkEvent,
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    }
                };
                eventInfos.Add(EventType.Branch, info);

                string explain0 = "파라미터의 조건으로 사용할 값 유형을 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam0, "Condition Type", ConditionType.LovePoint.ToString(), explain: explain0, options: Enum.GetNames(typeof(ConditionType))));
                string explain1 = "파라미터의 조건으로 사용할 값을 선택해주세요.";
                info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam1, "Condition Name", CharacterInfo.Seeun, explain: explain1, options: CharacterInfo.GetCharacterNames()));
                string explain2 = "첫 번째 브랜치가 요구하는 값을 입력해주세요.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam2, "Branch 1", 0.ToString(), explain: explain2, contentType: InputField.ContentType.DecimalNumber));
                //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam3, "Script ID 1", string.Empty));
                string explain3 = "두 번째 브랜치가 요구하는 값을 입력해주세요.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam4, "Branch 2", 0.ToString(), explain: explain3, contentType: InputField.ContentType.DecimalNumber));
                //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam5, "Script ID 2", string.Empty));
                string explain4 = "세 번째 브랜치가 요구하는 값을 입력해주세요.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam6, "Branch 3", 0.ToString(), explain: explain4, contentType: InputField.ContentType.DecimalNumber));
                //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam7, "Script ID 3", string.Empty));
                string explain5 = "네 번째 브랜치가 요구하는 값을 입력해주세요.";
                info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam8, "Branch 4", 0.ToString(), explain: explain5, contentType: InputField.ContentType.DecimalNumber));
                //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam7, "Script ID 4", string.Empty));
            }

            //EventInfo_Choice
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LinkEvent,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    }
                };
                eventInfos.Add(EventType.Choice, info);

                {
                    string explain0 = "첫 번째 선택지의 텍스트를 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam0, "Text 1", string.Empty, explain: explain0));
                    string explain1 = "첫 번째 파라미터의 조건으로 사용할 값 유형을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam0, "Condition Type 1", ConditionType.LovePoint.ToString(), explain: explain1, options: Enum.GetNames(typeof(ConditionType))));
                    string explain2 = "첫 번째 선택지가 파라미터의 조건으로 사용할 값을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam2, "Condition Name 1", CharacterInfo.Seeun, explain: explain2, options: StatInfo.GetStatNames()));
                    string explain3 = "첫 번째 선택지가 요구하는 값을 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam3, "Choice 1", 0.ToString(), explain: explain3, contentType: InputField.ContentType.DecimalNumber));
                    //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam4, "Script ID 1", ScriptVariable.autoTracking));

                    string explain5 = "두 번째 선택지의 텍스트를 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam5, "Text 2", string.Empty, explain: explain5));
                    string explain6 = "두 번째 파라미터의 조건으로 사용할 값 유형을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam6, "Condition Type 2", ConditionType.LovePoint.ToString(), explain: explain6, options: Enum.GetNames(typeof(ConditionType))));
                    string explain7 = "두 번째 선택지가 파라미터의 조건으로 사용할 값을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam7, "Condition Name 2", CharacterInfo.Seeun, explain: explain7, options: StatInfo.GetStatNames()));
                    string explain8 = "두 번째 선택지가 요구하는 값을 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam8, "Choice 2", 0.ToString(), explain: explain8, contentType: InputField.ContentType.DecimalNumber));
                    //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam9, "Script ID 2", ScriptVariable.autoTracking));

                    string explain10 = "세 번째 선택지의 텍스트를 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam10, "Text 3", string.Empty, explain: explain10));
                    string explain11 = "세 번째 파라미터의 조건으로 사용할 값 유형을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam11, "Condition Type 3", ConditionType.LovePoint.ToString(), explain: explain11, options: Enum.GetNames(typeof(ConditionType))));
                    string explain12 = "세 번째 선택지가 파라미터의 조건으로 사용할 값을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam12, "Condition Name 3", CharacterInfo.Seeun, explain: explain12, options: StatInfo.GetStatNames()));
                    string explain13 = "세 번째 선택지가 요구하는 값을 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam13, "Choice 3", 0.ToString(), explain: explain13, contentType: InputField.ContentType.DecimalNumber));
                    //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam14, "Script ID 3", ScriptVariable.autoTracking));

                    string explain15 = "두 번째 선택지의 텍스트를 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam15, "Text 4", string.Empty, explain: explain15));
                    string explain16 = "두 번째 파라미터의 조건으로 사용할 값 유형을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam16, "Condition Type 4", ConditionType.LovePoint.ToString(), explain: explain16, options: Enum.GetNames(typeof(ConditionType))));
                    string explain17 = "두 번째 선택지가 파라미터의 조건으로 사용할 값을 선택해주세요.";
                    info.paramInfo.Add(new(VariableType.Dropdown, ScriptDataKey.EventParam17, "Condition Name 4", CharacterInfo.Seeun, explain: explain17, options: StatInfo.GetStatNames()));
                    string explain18 = "두 번째 선택지가 요구하는 값을 입력해주세요.";
                    info.paramInfo.Add(new(VariableType.InputField, ScriptDataKey.EventParam18, "Choice 4", 0.ToString(), explain: explain18, contentType: InputField.ContentType.DecimalNumber));
                    //info.paramInfo.Add(new(VariableType.Script, ScriptDataKey.EventParam19, "Script ID 4", ScriptVariable.autoTracking));
                }
            }

            //EventInfo_CloseScenario
            {
                ScriptInfo info = new()
                {
                    excludedKeys = new()
                    {
                        ScriptDataKey.LinkEvent,
                        ScriptDataKey.EventDuration,
                        ScriptDataKey.DurationTurn,
                        ScriptDataKey.LoopCount,
                        ScriptDataKey.LoopDelay,
                        ScriptDataKey.LoopType,
                    }
                };
                eventInfos.Add(EventType.CloseScenario, info);
            }
        }
    }
}
