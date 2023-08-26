using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScriptInspector : Singleton<ScriptInspector>
{
    [SerializeField] private List<GameObject> variablePref;

    private NodeGraph nodeGrp;

    private List<Variable> variables;


    private List<ScriptDataKey> publicVaraibleForEvent = new()
    {
        ScriptDataKey.EventType,

        ScriptDataKey.LinkEvent,

        ScriptDataKey.SkipMethod,
        ScriptDataKey.SkipDelay,

        ScriptDataKey.EventDelay,

        ScriptDataKey.EventDuration,

        ScriptDataKey.DurationTurn,

        ScriptDataKey.LoopCount,
        ScriptDataKey.LoopType,
        ScriptDataKey.LoopDelay,
    };

    private void Awake()
    {
        ScriptInfo.Init();
    }

    private void Start()
    {
        nodeGrp = NodeGraph.Instance;
    }

    public void SetVaraibles(Node node)
    {
        transform.DestroyAllChildren();

        variables = new();

        if(node == null) { return; }

        if (node.script.scriptType == ScriptType.Text)
        {
            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.LinkEvent]);
            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.SkipMethod]);
            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.SkipDelay]);

            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.CharacterName]);
            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.TextDuration]);
            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.Audio0]);
            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.Audio1]);
            CreateVariable(node, ScriptInfo.scriptParamInfos[ScriptDataKey.Text]);
        }
        else if(node.script.scriptType == ScriptType.Event)
        {
            CreateVariable(node, ScriptInfo.eventInfos[node.script.eventData.eventType]);
        }
    }

    public void ApplyAllVariables()
    {
        foreach(var variable in variables)
        {
            variable.ApplyValue();
        }
    }

    private Variable CreateVariable(Node node, VariableType varType, ScriptDataKey key, string varName, string explain = null, InputField.ContentType contentType = InputField.ContentType.Standard, string[] options = null)
    {
        Variable var = Instantiate(variablePref[(int)varType]).GetComponent<Variable>();

        var.transform.SetParent(transform, false);

        string value = node.script.GetVariableFromKey(key);

        var.Init(node, key, varName, value, contentType, options);

        variables.Add(var);

        return var;
    }

    private List<Variable> CreateVariable(Node node, ScriptInfo scriptInfo)
    {
        List<Variable> variables = new List<Variable>();

        List<ScriptDataKey> varList = publicVaraibleForEvent.ToList();

        //예외 키를 리스트에서 삭제
        foreach(var excludedKey in scriptInfo.excludedKeys)
        {
            varList.Remove(excludedKey);
        }

        //공용 바리에이블 생성
        foreach(var key in varList)
        {
            var variable = CreateVariable(node, ScriptInfo.scriptParamInfos[key]);
            variables.Add(variable);
        }

        foreach(var param in scriptInfo.paramInfo)
        {
            var variable = CreateVariable(node, param.varType, param.targetKey, param.paramName, param.explain, param.contentType, param.options);
            variables.Add(variable);
        }

        return variables;
    }

    private Variable CreateVariable(Node node, ParamInfo paramInfo)
    {
        Variable var = CreateVariable(node, paramInfo.varType, paramInfo.targetKey, paramInfo.paramName, paramInfo.explain, paramInfo.contentType, paramInfo.options);

        return var;
    }
}
