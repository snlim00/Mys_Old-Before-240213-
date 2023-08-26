using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptVariable : Variable
{
    [SerializeField] private Button button;
    [SerializeField] private Button autoTrackingButton;
    [SerializeField] private Text value;

    public const string autoTracking = "Auto";

    public override void Init(Node node, ScriptDataKey targetKey, string varName, string value, InputField.ContentType contentType, string[] dropdownOptions = null, Variable parent = null)
    {
        base.Init(node, targetKey, varName, value, contentType, dropdownOptions, parent);

        button.onClick.AddListener(() =>
        {
            if(nodeGrp.inputType == InputType.Script) { return; }

            nodeGrp.inputType = InputType.Script;
            nodeGrp.OnSelectScript += (node) =>
            {
                SetValue(node.script.scriptId.ToString());
                ApplyValue();
            };
        });

        if(node.script.scriptType == ScriptType.Event && node.script.eventData.eventType == EventType.BranchEnd)
        {
            autoTrackingButton.gameObject.SetActive(true);

            autoTrackingButton.onClick.AddListener(() =>
            {
                SetValue(autoTracking);
            });
        }
    }

    public override void SetValue(string value)
    {
        this.value.text = value;
    }

    public override string GetValue()
    {
        return value.text;
    }
}
