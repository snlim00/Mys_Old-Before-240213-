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
    Text,
    Object,
    //Node,
}

public class Variable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text varName;
    [SerializeField] private InputField inputField;
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private Button button;

    private NodeGraph nodeGrp;
    private ScriptInspector inspector;
    private VariableTooltip varTooltip;

    public EventParamInfo eventParamInfo = null;

    public Node targetNode;
    public ScriptDataKey targetKey;

    public VariableType type = VariableType.InputField;

    private void Awake()
    {
        nodeGrp = NodeGraph.instance;
        inspector = ScriptInspector.instance;
        varTooltip = VariableTooltip.instance;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if(e.pointerEnter.CompareTag(Tag.VariableName) == false)
        {
            return;
        }

        if(eventParamInfo != null && eventParamInfo.explain != null)
        {
            varTooltip.ShowTooltip(eventParamInfo.explain);
        }
    }
    
    public void OnPointerExit(PointerEventData e)
    {
        varTooltip.HideTooltip();
    }

    public void Init(VariableType type)
    {
        this.type = type;

        switch(type)
        {
            case VariableType.Dropdown:
                dropdown.gameObject.SetActive(true);
                break;

            case VariableType.InputField:
                inputField.gameObject.SetActive(true);
                break;

            case VariableType.Text:
                inputField.gameObject.SetActive(true);

                GetComponent<RectTransform>().sizeDelta += new Vector2(0, 70);
                inputField.GetComponent<RectTransform>().sizeDelta += new Vector2(435, 70);
                inputField.transform.localPosition += new Vector3(435 / 2, 0);
                break;

            case VariableType.Object:
                button.gameObject.SetActive(true);

                var stream = Observable.EveryUpdate();
                
                button.onClick.AddListener(() =>
                {
                    OnClickObjectBtn();
                });
                break;
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
            "이벤트 타입 변경".로그();
            inspector.RefreshInspector(targetNode);

            //string value = GetValue();
            //EventType eventType = (EventType)Enum.Parse(typeof(EventType), value);

            targetNode.Refresh();
        }
    }

    public void OnEndEdit()
    {
        nodeGrp.Save();
    }

    private void OnClickObjectBtn()
    {
        button.image.color = Color.gray;

        Observable.TimerFrame(1).Subscribe(_ =>
        {
            IDisposable stream = null;
            stream = Observable.EveryUpdate()
                .Select(_ => EventSystem.current.currentSelectedGameObject)
                .DistinctUntilChanged()
                .Subscribe(_ =>
                {
                    GameObject current = EventSystem.current.currentSelectedGameObject;

                    if (current == button.gameObject)
                    {
                        return;
                    }

                    button.image.color = Color.white;

                    if (current.CompareTag(Tag.CharacterList) == false)
                    {
                        return;
                    }

                    SetValue(current.GetComponent<Button>().GetButtonText());

                    nodeGrp.Save();

                    stream.Dispose();
                });
        });
    }

    public void SetDropdownOption(string[] options)
    {
        if(type != VariableType.Dropdown)
        {
            "Varaible Type이 Dropdown이 아닙니다".LogWarning(this.name);
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
        this.name = name;
    }

    public void SetValue(string value)
    {
        if (type == VariableType.InputField || type == VariableType.Text)
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
        else if(type == VariableType.Object)
        {
            button.SetButtonText(value);
        }
    }

    public string GetValue()
    {
        switch(type)
        {
            case VariableType.InputField:
            case VariableType.Text:
                return inputField.text;

            case VariableType.Dropdown:
                var options = dropdown.options;
                int value = dropdown.value;

                return options[value].text;

            case VariableType.Object:
                return button.GetButtonText();

            default:
                return null;
        }
    }

    public void ApplyValue()
    {
        targetNode.script.SetVariable(targetKey, GetValue());
    }
}
