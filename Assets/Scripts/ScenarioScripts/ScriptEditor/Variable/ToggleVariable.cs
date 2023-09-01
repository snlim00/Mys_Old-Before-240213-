using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleVariable : Variable
{
    [SerializeField] private Toggle toggle;

    public override void Init(Node node, ScriptDataKey targetKey, string varName, string value, InputField.ContentType contentType, string[] dropdownOptions = null, Variable parent = null)
    {
        base.Init(node, targetKey, varName, value, contentType, dropdownOptions, parent);

        toggle.onValueChanged.AddListener((value) =>
        {
            ApplyValue();

            if(targetKey == ScriptDataKey.LinkEvent)
            {
                nodeGrp.RefreshAll();
            }
        });
    }

    public override void SetValue(string value)
    {
        if(bool.TryParse(value, out bool isOn))
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }
        else
        {
            toggle.SetIsOnWithoutNotify(false);
        }
    }

    public override string GetValue()
    {
        return toggle.isOn.ToString();
    }
}
