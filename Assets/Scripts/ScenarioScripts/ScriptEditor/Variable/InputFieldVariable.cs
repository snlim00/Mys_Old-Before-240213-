using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldVariable : Variable
{
    [SerializeField] private InputField inputField;

    public override void Init(Node node, ScriptDataKey targetKey, string varName, string value, InputField.ContentType contentType, string[] dropdownOption = null, Variable parent = null)
    {
        base.Init(node, targetKey, varName, value, contentType, dropdownOption);

        inputField.contentType = contentType;

        var trigger = inputField.gameObject.GetComponent<EventTrigger>();
        var onSelect = new EventTrigger.Entry();

        onSelect.callback.AddListener(_ =>
        {
            nodeGrp.inputType = InputType.EditInputField;
        });
        trigger.triggers.Add(onSelect);

        inputField.onEndEdit.AddListener((value) =>
        {
            ApplyValue();
            nodeGrp.inputType = InputType.Select;
        });
    }

    public override void SetValue(string value)
    {
        inputField.text = value;
        inputField.SetTextWithoutNotify(value);
    }

    public override string GetValue()
    {
        return inputField.text;
    }
}
