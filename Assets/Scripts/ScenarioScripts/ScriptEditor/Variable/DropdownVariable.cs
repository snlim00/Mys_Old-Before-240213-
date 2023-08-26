using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownVariable : Variable
{
    [SerializeField] private Dropdown dropdown;

    public override void Init(Node node, ScriptDataKey targetKey, string varName, string value, InputField.ContentType contentType, string[] dropdownOptions = null, Variable parent = null)
    {
        SetOptions(dropdownOptions);

        base.Init(node, targetKey, varName, value, contentType, dropdownOptions);

        dropdown.onValueChanged.AddListener((value) =>
        {
            ApplyValue();

            if(node.script.scriptType == ScriptType.Event && targetKey == ScriptDataKey.EventType)
            {
                nodeGrp.SetInspector();
                targetNode.Refresh();
            }
        });
    }

    public void SetOptions(string[] options)
    {
        dropdown.ClearOptions();

        List<Dropdown.OptionData> optionList = new();

        foreach (string option in options)
        {
            optionList.Add(new(option));
        }

        dropdown.AddOptions(optionList);
    }

    public override void SetValue(string value)
    {
        for(int i = 0; i < dropdown.options.Count; ++i)
        {
            if (dropdown.options[i].text == value)
            {
                dropdown.SetValueWithoutNotify(i);

                break;
            }
        }
    }

    public override string GetValue()
    {
        return dropdown.options[dropdown.value].text;
    }
}
