using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum VariableType
{
    InputField,
    Dropdown,
    Toggle,
    Condition,
    Object,
    Script
}

public abstract class Variable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Text variableName;

    protected NodeGraph nodeGrp;
    protected ScriptInspector inspector;
    protected VariableTooltip tooltip;

    protected Node targetNode;
    protected ScriptDataKey targetKey;

    private void Start()
    {
        tooltip = VariableTooltip.Instance;
    }

    public virtual void Init(Node node, ScriptDataKey targetKey, string varName, string value, InputField.ContentType contentType, string[] dropdownOptions = null, Variable parent = null)
    {
        targetNode = node;
        this.targetKey = targetKey;

        nodeGrp = NodeGraph.Instance;
        inspector = ScriptInspector.Instance;

        variableName.text = varName;
        SetValue(value);
    }

    public void ApplyValue()
    {
        targetNode.script.SetVariable(targetKey, GetValue());
    }

    public void OnPointerEnter(PointerEventData e)
    {
        "OnPointerEnter".Log();
        EventData eventData = targetNode.script.eventData;

        string explain = ScriptInfo.eventInfos[eventData.eventType].GetParamInfo(targetKey)?.explain;

        explain.Log();

        if(explain != null)
        {
            tooltip.ShowTooltip(explain);
        }
    }

    public void OnPointerExit(PointerEventData e)
    {
        "OnPointerExit".Log();
        tooltip.HideTooltip();
    }

    public abstract void SetValue(string value);

    public abstract string GetValue();
}
