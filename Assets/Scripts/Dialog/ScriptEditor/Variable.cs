using DG.Tweening.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum VariableType
{
    InputField,
    Dropdown,
    //Character,
    //Node,
}

public class Variable : MonoBehaviour
{
    [SerializeField] private Text varName;
    [SerializeField] private InputField inputField;
    [SerializeField] private Dropdown dropdown;

    private NodeGraph nodeGrp;
    private ScriptInspector inspector;

    public EventParamInfo eventParamInfo = null;

    public Node targetNode;
    public ScriptDataKey targetKey;

    public VariableType type = VariableType.InputField;

    private void Awake()
    {
        nodeGrp = NodeGraph.instance;
        inspector = ScriptInspector.instance;
    }

    public void Init(VariableType type)
    {
        this.type = type;

        switch(type)
        {
            case VariableType.Dropdown:
                inputField.gameObject.SetActive(false);
                dropdown.gameObject.SetActive(true);
                break;

            case VariableType.InputField:
                inputField.gameObject.SetActive(true);
                dropdown.gameObject.SetActive(false);
                break;

            //case VariableType.Character:
            //    inputField.gameObject.SetActive(true);
            //    dropdown.gameObject.SetActive(false);
            //    break;
        }

        transform.SetParent(inspector.transform); 

        transform.localScale = Vector3.one;

        dropdown.onValueChanged.AddListener(_ => OnValueChange());
        dropdown.onValueChanged.AddListener(_ => nodeGrp.Save());
        
        inputField.onValueChanged.AddListener(_ => OnValueChange());
        inputField.onEndEdit.AddListener(_ => nodeGrp.Save());
    }


    public void OnValueChange()
    {
        if (targetKey == ScriptDataKey.EventType)
        {
            //"이벤트 타입 변경".로그();
            inspector.RefreshInspector(targetNode);

            string value = GetValue();
            EventType eventType = (EventType)Enum.Parse(typeof(EventType), value);

            if (eventType == EventType.Branch)
            {
                targetNode.SetNodeType(Node.NodeType.Branch);
            }
            else if (eventType == EventType.Goto)
            {
                targetNode.SetNodeType(Node.NodeType.Goto);
            }
            else
            {
                targetNode.SetNodeType(Node.NodeType.Normal);
            }
        }
    }

    public void OnEndEdit()
    {
        nodeGrp.Save();
    }

    public void SetDropdownOption(string[] options)
    {
        if(type != VariableType.Dropdown)
        {
            "Varaible Type이 Dropdown이 아닙니다".로그();
            return;
        }

        dropdown.ClearOptions();

        List<Dropdown.OptionData> optionList = new();

        foreach(string option in options)
        {
            optionList.Add(new(option));
        }

        dropdown.AddOptions(optionList);
    }

    public void SetContentType(InputField.ContentType contentType)
    {
        inputField.contentType = contentType;
    }

    public void SetTarget(Node targetNode, ScriptDataKey targetKey)
    {
        this.targetNode = targetNode;
        this.targetKey = targetKey;
    }

    public void SetName(string name)
    {
        varName.text = name;
    }

    public void SetValue(string value)
    {

        if (type == VariableType.InputField)
        {
            inputField.SetTextWithoutNotify(value);
        }
        else if (type == VariableType.Dropdown)
        {
            var options = dropdown.options;

            for(int i = 0; i < options.Count; ++i)
            {
                if (options[i].text == value)
                {
                    dropdown.SetValueWithoutNotify(i);

                    break;
                }
            }
        }
    }

    public string GetValue()
    {
        switch(type)
        {
            case VariableType.InputField:
                return inputField.text;

            case VariableType.Dropdown:
                var options = dropdown.options;
                int value = dropdown.value;

                return options[value].text;

            default:
                return null;
        }
    }

    public void ApplyValue()
    {
        targetNode.script.SetVariable(targetKey, GetValue());
    }
}
