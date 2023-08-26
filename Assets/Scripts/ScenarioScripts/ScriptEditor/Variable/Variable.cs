using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum VariableType
{
    InputField,
    Dropdown,
    Toggle,
    Condition,
    Object,
    Script
}

public abstract class Variable : MonoBehaviour
{
    [SerializeField] protected Text variableName;

    protected NodeGraph nodeGrp;
    protected ScriptInspector inspector;

    protected Node targetNode;
    protected ScriptDataKey targetKey;

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

    public abstract void SetValue(string value);

    public abstract string GetValue();
}
