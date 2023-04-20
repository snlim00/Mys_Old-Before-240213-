using DG.Tweening;
using System.Collections;
using System;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    private Ease defaultEase = Ease.Linear;

    private DialogManager dialogMgr;
    private TextManager textMgr;

    public Transform[] objectPositions;
    private GameObject objectPref;
    private Dictionary<string, MysObject> ObjectList;
    [SerializeField] private Transform objectParent;

    private GameObject choiceOptionPref;
    private List<Button> choiceOptionList = new();
    [SerializeField] private Transform choiceOptionParent;

    [SerializeField] private Image background;
    [SerializeField] private Sprite defaultBG;

    private void Awake()
    {
        ObjectList = new();

        objectPref = Resources.Load<GameObject>("Prefabs/CharacterPref");
        choiceOptionPref = Resources.Load<GameObject>("Prefabs/ChoiceOption");
        textMgr = FindObjectOfType<TextManager>();
    }

    private void Start()
    {
        dialogMgr = DialogManager.instance;
    }

    public Sequence CreateEventSequence(ScriptObject script)
    {
        Sequence seq = DOTween.Sequence();

        //선딜 처리
        if (script.eventData.eventDelay > 0)
        {
            seq.AppendInterval(script.eventData.eventDelay);
        }

        CallEvent(script, ref seq);

        //루프 처리
        if (script.eventData.loopCount != 0)
        {
            if (script.eventData.loopDelay != 0)
            {
                seq.AppendInterval(script.eventData.loopDelay);
            }

            seq.SetLoops(script.eventData.loopCount, script.eventData.loopType);
        }

        return seq;
    }


    public void CallEvent(ScriptObject script, ref Sequence sequence)
    {
        switch(script.eventData.eventType)
        {
            case EventType.None:
                ("이벤트가 존재하지 않습니다. ScriptID : " + script.scriptID).LogError();
                break;

            case EventType.CreateObject:
                Event_CreateObject(script, ref sequence);
                break;

            case EventType.MoveObject:
                Event_MoveObject(script, ref sequence);
                break;

            case EventType.RemoveObject:
                Event_RemoveObject(script, ref sequence);
                break;

            case EventType.RemoveAllObject:
                Event_RemoveAllObject(script, ref sequence);
                break;

            case EventType.HideTextBox:
                Event_HideTextBox(script, ref sequence);
                break;

            case EventType.Goto:
                Event_Goto(script, ref sequence);
                break;

            case EventType.Branch:
                Event_Branch(script, ref sequence);
                break;

            case EventType.SetBackground:
                Event_SetBackground(script, ref sequence);
                break;

            case EventType.Choice:
                Event_Choice(script, ref sequence);
                break;
        }
    }

    public void Event_CreateObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string resource = eventData.eventParam[0];
        string name = eventData.eventParam[1];
        int position = int.Parse(eventData.eventParam[2]);
        
        Sprite sprite = Resources.Load<Sprite>("Images/Character/" + resource);

        MysObject obj = Instantiate(objectPref).GetComponent<MysObject>();
        obj.transform.SetParent(objectParent);
        ObjectList[name] = obj;

        obj.image.sprite = sprite;
        obj.image.SetAlpha(0);
        obj.name = name;

        void CreateObject()
        {
            obj.SetPosition(position);

            obj.image.SetAlpha(0);
        }

        sequence.AppendCallback(CreateObject);
        sequence.Append(obj.image.DOFade(1, eventData.eventDuration));
    }

    public void Event_MoveObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0];
        int position = int.Parse(eventData.eventParam[1]);
        float duration = int.Parse(eventData.eventParam[2]);

        Ease ease = DOTween.defaultEaseType;
        Enum.TryParse(typeof(Ease), eventData.eventParam[3], out object outValue);
        if(outValue != null)
        {
            ease = (Ease)outValue;

            if(ease == Ease.Unset)
            {
                ease = defaultEase;
            }
        }

        var obj = ObjectList[name];

        sequence.Append(obj.transform.DOMove(objectPositions[position].position, duration).SetEase(ease));
    }

    public void RemoveObject(string name)
    {
        MysObject character = ObjectList[name];

        ObjectList.Remove(name);

        Destroy(character.gameObject);
    }

    public void Event_RemoveObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0].ToString();

        MysObject character = ObjectList[name];

        sequence.Append(character.image.DOFade(0, eventData.eventDuration));
        sequence.AppendCallback(() => RemoveObject(name));
    }

    public void RemoveAllObject()
    {
        Dictionary<string, MysObject>.KeyCollection keys = ObjectList.Keys;

        List<string> keyList = new();

        foreach(string key in keys)
        {
            keyList.Add(key);
        }

        for(int i = 0; i < keyList.Count; ++i)
        {
            RemoveObject(keyList[i]);
        }
    }

    public void Event_RemoveAllObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        sequence.AppendCallback(RemoveAllObject);
    }

    public void Event_HideTextBox(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        bool hide = bool.Parse(eventData.eventParam[0]);

        float targetAlpha = hide == true ? 0 : 1;

        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.Append(textMgr.text.DOFade(targetAlpha, eventData.eventDuration));
        fadeSequence.Insert(0, textMgr.textBox.DOFade(targetAlpha, eventData.eventDuration));
        fadeSequence.Insert(0, textMgr.characterName.DOFade(targetAlpha, eventData.eventDuration));

        sequence.Append(fadeSequence);
    }

    public void Event_AddLovePoint(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string target = eventData.eventParam[0];
        int amount = int.Parse(eventData.eventParam[1]);

        sequence.AppendCallback(() => ProgressData.AddLovePoint(script, target, amount));
    }

    public void Goto(int scriptID)
    {
        dialogMgr.Goto(scriptID);
    }

    public void Event_Goto(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;
        int scriptID = int.Parse(eventData.eventParam[0]);

        sequence.AppendCallback(() => Goto(scriptID));
        Goto(scriptID);
    }

    public void SetBackground(Sprite sprite)
    {
        if(sprite != null)
        {
            background.sprite = sprite;
        }
        else
        {
            background.sprite = defaultBG;
        }
    }

    public void Event_SetBackground(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string resource = eventData.eventParam[0];

        Sprite sprite = Resources.Load<Sprite>("Images/Background/" + resource);

        sequence.Append(background.DOFade(0, eventData.eventDuration / 2));
        sequence.AppendCallback(() => SetBackground(sprite));
        sequence.Append(background.DOFade(1, eventData.eventDuration / 2));
    }

    public void Event_Branch(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string target = eventData.eventParam[0];
        int lovePoint = ProgressData.GetLovePointFromCharacter(script, target);

        BranchInfo branchInfo = BranchInfo.GetBranchInfo(script);

        int branch = -1;

        for (int i = branchInfo.Count - 1; i >= 0; --i)
        {
            if (branchInfo.requiredValue[i] <= lovePoint)
            {
                branch = i;
                break;
            }
        }

        if (branch == -1)
        {
            (script.scriptID).LogError("분기를 찾을 수 없습니다. ScriptID", true);
            return;
        }

        int scriptID = branchInfo.targetID[branch];

        sequence.AppendCallback(() => Goto(scriptID));
    }

    public void RemoveAllChoiceOption(float duration)
    {
        foreach (var btn in choiceOptionList)
        {
            if (btn == null) break;

            Sequence seq = DOTween.Sequence();
            seq.Append(btn.image.DOFade(0, duration));
            seq.Insert(0, btn.GetComponentInChildren<Text>().DOFade(0, duration));
            seq.AppendCallback(() => Destroy(btn.gameObject));

            seq.Play();
        }
    }

    public void Event_Choice(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;
        
        Button CreateChoiceBtn(string choiceText, int targetID)
        {
            Button btn = Instantiate(choiceOptionPref).GetComponent<Button>();

            btn.transform.SetParent(choiceOptionParent);
            btn.transform.localScale = Vector3.one;
            choiceOptionList.Add(btn);

            btn.SetButtonText(choiceText);

            //AddListener
            btn.onClick.AddListener(() =>
            {
                Goto(targetID);

                RemoveAllChoiceOption(eventData.eventDuration);
            });

            Text text = btn.GetComponentInChildren<Text>();

            btn.image.SetAlpha(0);
            text.SetAlpha(0);

            Sequence seq = DOTween.Sequence();
            seq.Append(btn.image.DOFade(1, eventData.eventDuration));
            seq.Insert(0, text.DOFade(1, eventData.eventDuration));

            seq.Play();

            return btn;
        }

        choiceOptionList = new();
       
        BranchInfo branchInfo = BranchInfo.GetBranchInfo(script);
        branchInfo.Log();

        sequence.AppendCallback(() =>
        {
            dialogMgr.DisposeSkipStream();

            for (int i = 0; i < branchInfo.Count; ++i)
            {
                Button btn = CreateChoiceBtn(branchInfo.choiceText[i], branchInfo.targetID[i]);
            }

        });
    }
}
