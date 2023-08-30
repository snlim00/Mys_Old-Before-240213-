using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ObjectVariable : Variable
{
    [SerializeField] private Button button;
    [SerializeField] private Text value;

    public override void Init(Node node, ScriptDataKey targetKey, string varName, string value, InputField.ContentType contentType, string[] dropdownOptions = null, Variable parent = null)
    {
        base.Init(node, targetKey, varName, value, contentType, dropdownOptions, parent);

        button.onClick.AddListener(() =>
        {
            if (nodeGrp.inputType == InputType.Object) { return; }

            button.image.color = Color.gray;

            var stream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .Take(1)
                .Subscribe(_ => {
                    nodeGrp.CancelSelectScript();
                    button.image.color = Color.white;
                    });

            nodeGrp.inputType = InputType.Object;
            nodeGrp.OnSelectObject += (name) =>
            {
                SetValue(name);
                ApplyValue();

                button.image.color = Color.white;
                stream.Dispose();
            };
        });
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
