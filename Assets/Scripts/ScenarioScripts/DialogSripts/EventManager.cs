using DG.Tweening;
using System.Collections;
using System;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    private Ease defaultEase = Ease.Linear;

    private DialogManager dialogMgr;
    private TextManager textMgr;

    [SerializeField] private Image fadeObj;

    [SerializeField] private GameObject objectPref;
    private Dictionary<string, DialogObject> objectList;
    [SerializeField] private Transform objectParent;

    [SerializeField] private GameObject choiceOptionPref;
    private List<ChoiceOptionButton> choiceOptionList = new();
    [SerializeField] private Transform choiceOptionParent;

    [SerializeField] private Image background;
    [SerializeField] private Sprite defaultBg;

    [SerializeField] private Image titleImg;
    [SerializeField] private Text title;
    [SerializeField] private Text subtitle;

    [SerializeField] private Image cgImg;

    private void Awake()
    {
        objectList = new();

        textMgr = FindObjectOfType<TextManager>();
    }

    private void Start()
    {
        dialogMgr = DialogManager.Instance;
    }

    public Sequence CreateEventSequence(ScriptObject script)
    {
        Sequence seq = DOTween.Sequence();

        //딜레이 처리
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
        switch (script.eventData.eventType)
        {
            case EventType.None:
                ("이벤트가 존재하지 않습니다. ScriptID : " + script.scriptId).LogError();
                break;

            case EventType.SetBackground:
                Event_SetBackground(script, ref sequence);
                break;

            case EventType.PlayBGM:
                Event_PlayBGM(script, ref sequence);
                break;

            case EventType.FadeIn:
                Event_FadeIn(script, ref sequence);
                break;

            case EventType.CreateObject:
                Event_CreateObject(script, ref sequence);
                break;

            case EventType.MoveObject:
                Event_MoveObject(script, ref sequence);
                break;

            case EventType.FlipObject:
                Event_FlipObject(script, ref sequence);
                break;

            case EventType.SetObjectAlpha:
                Event_SetObjectAlpha(script, ref sequence);
                break;

            case EventType.SetObjectImage:
                Event_SetObjectImage(script, ref sequence);
                break;

            case EventType.SetObjectScale:
                Event_SetObjectScale(script, ref sequence);
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

            case EventType.ShowTitle:
                Event_ShowTitle(script, ref sequence);
                break;

            case EventType.ShowCg:
                Event_ShowCg(script, ref sequence);
                break;

            case EventType.Goto:
            case EventType.BranchEnd:
                Event_Goto(script, ref sequence);
                break;

            case EventType.Branch:
                Event_Branch(script, ref sequence);
                break;

            case EventType.Choice:
                Event_Choice(script, ref sequence);
                break;

            case EventType.CloseScenario:
                Event_CloseScenario(script, ref sequence);
                break;
        }
    }

    public void ResetAll()
    {
        background.sprite = defaultBg;
        titleImg.SetAlpha(0);
        title.SetAlpha(0);
        subtitle.SetAlpha(0);
        cgImg.sprite = defaultBg;
        cgImg.SetAlpha(0);

        foreach(var obj in objectList)
        {
            Destroy(obj.Value?.gameObject);
        }
        objectList = new();
    }

    public void SetBackground(Sprite sprite)
    {
        if (sprite != null)
        {
            background.sprite = sprite;
        }
        else
        {
            background.sprite = defaultBg;
        }
    }

    public void Event_SetBackground(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string resource = eventData.eventParam[0];

        Sprite sprite = Resources.Load<Sprite>(PathManager.CreateBackgroundImagePath(resource));

        sequence.Append(background.DOFade(0, eventData.eventDuration / 2));
        sequence.AppendCallback(() => SetBackground(sprite));
        sequence.Append(background.DOFade(1, eventData.eventDuration / 2));
    }

    public void Event_PlayBGM(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string resource = eventData.eventParam[0];

        AudioClip clip = Resources.Load<AudioClip>(PathManager.CreateBGMPath(resource));

        PathManager.CreateBGMPath(resource).Log();
        clip.Log();

        sequence.AppendCallback(() => SoundManager.PlayBGM(clip));
    }

    public void Event_FadeIn(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        bool fadeIn = bool.Parse(eventData.eventParam[0]);
        bool autoFadeOut = bool.Parse(eventData.eventParam[1]);
        float duration = float.Parse(eventData.eventParam[2]);

        int alpha = fadeIn == true ? 1 : 0;
        if (fadeIn == false) { autoFadeOut = false; }

        sequence.Append(fadeObj.DOFade(alpha, eventData.eventDuration));

        if (autoFadeOut == true)
        {
            sequence.AppendInterval(duration);

            sequence.Append(fadeObj.DOFade(0, eventData.eventDuration));
        }
    }

    public void Event_CreateObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0];
        string resource = eventData.eventParam[1];
        DialogObjectTag tag = (DialogObjectTag)Enum.Parse(typeof(DialogObjectTag), eventData.eventParam[2]);
        float position = float.Parse(eventData.eventParam[3]);
        int headPosition = 0;
        if (tag == DialogObjectTag.Object)
        {
            headPosition = int.Parse(eventData.eventParam[4]);
        }
        else
        {
            headPosition = CharacterInfo.GetHeadPosition(tag);
        }

        Sprite sprite = Resources.Load<Sprite>(PathManager.CreateImagePath(resource));

        DialogObject obj = Instantiate(objectPref).GetComponent<DialogObject>();

        sequence.AppendCallback(() =>
        {
            obj.transform.SetParent(objectParent, false);

            objectList[name] = obj;

            obj.Init(sprite, name, position, headPosition);

            obj.image.SetAlpha(0);
        });

        sequence.Append(obj.image.DOFade(1, eventData.eventDuration));
    }

    public void Event_MoveObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0];
        int position = int.Parse(eventData.eventParam[1]);

        Ease ease = DOTween.defaultEaseType;
        Enum.TryParse(typeof(Ease), eventData.eventParam[3], out object outValue);
        if (outValue != null)
        {
            ease = (Ease)outValue;

            if (ease == Ease.Unset)
            {
                ease = defaultEase;
            }
        }

        var obj = objectList[name];

        sequence.Append(obj.MoveObject(position, eventData.eventDuration, ease));
    }

    public void Event_FlipObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0];

        sequence.AppendCallback(() =>
        {
            DialogObject obj = objectList[name];

            Vector3 scale = obj.transform.localScale;

            scale.x *= -1;

            obj.transform.localScale = scale;
        });
    }

    public void Event_SetObjectAlpha(ScriptObject script, ref Sequence sequence)
    {

        EventData eventData = script.eventData;

        string name = eventData.eventParam[0];
        float alpha = float.Parse(eventData.eventParam[1]);

        DialogObject obj = objectList[name];

        sequence.Append(obj.image.DOFade(alpha, eventData.eventDuration));
    }

    public void Event_SetObjectImage(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0];
        string resource = eventData.eventParam[1];
        bool doFade = bool.Parse(eventData.eventParam[2]);
        float fadeDuration = float.Parse(eventData.eventParam[3]);

        DialogObject obj = objectList[name];
        Sprite sprite = Resources.Load<Sprite>(PathManager.CreateImagePath(resource));

        if(doFade == false)
        {
            sequence.AppendCallback(() => obj.SetSprite(sprite));
        }
        else
        {
            sequence.AppendCallback(() =>
            {
                obj.SetSubSprite(obj.image.sprite);
                obj.SetSprite(sprite);
                obj.subImage.SetAlpha(1);
            });
            sequence.Append(obj.subImage.DOFade(0, fadeDuration));
        }
    }

    public void Event_SetObjectScale(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0];
        float scale = float.Parse(eventData.eventParam[1]);
        bool fittingTop = bool.Parse(eventData.eventParam[2]);

        sequence.AppendCallback(() =>
        {
            DialogObject obj = objectList[name];
            obj.SetScale(scale, fittingTop);
        });
    }

    public void RemoveObject(string name)
    {
        DialogObject character = objectList[name];

        objectList.Remove(name);

        Destroy(character.gameObject);
    }

    public void Event_RemoveObject(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string name = eventData.eventParam[0].ToString();

        DialogObject character = objectList[name];

        sequence.Append(character.image.DOFade(0, eventData.eventDuration));
        sequence.AppendCallback(() => RemoveObject(name));
    }

    public void RemoveAllObject()
    {
        Dictionary<string, DialogObject>.KeyCollection keys = objectList.Keys;

        List<string> keyList = new();

        foreach (string key in keys)
        {
            keyList.Add(key);
        }

        for (int i = 0; i < keyList.Count; ++i)
        {
            objectList[keyList[i]].objName.Log();
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

        float targetAlpha = hide == true ? 0 : 1f;

        sequence.Append(textMgr.text.DOFade(targetAlpha, eventData.eventDuration));
        sequence.Insert(0, textMgr.textBox.DOFade(targetAlpha, eventData.eventDuration));
        sequence.Insert(0, textMgr.characterName.DOFade(targetAlpha, eventData.eventDuration));
    }

    public void Event_ShowTitle(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        bool show = bool.Parse(eventData.eventParam[0]);
        bool autoHide = bool.Parse(eventData.eventParam[1]);
        float duration = float.Parse(eventData.eventParam[2]);
        string title = eventData.eventParam[3];
        string subtitle = eventData.eventParam[4];

        int alpha = show == true ? 1 : 0;
        if(show == false) { autoHide = false; }

        if(show == true)
        {
            this.title.text = title;
            this.subtitle.text = subtitle;
        }

        sequence.Append(titleImg.DOFade(alpha, eventData.eventDuration));
        sequence.Join(this.title.DOFade(alpha, eventData.eventDuration));
        sequence.Join(this.subtitle.DOFade(alpha, eventData.eventDuration));

        if(autoHide == true)
        {
            sequence.AppendInterval(duration);

            sequence.Append(titleImg.DOFade(0, eventData.eventDuration));
            sequence.Join(this.title.DOFade(0, eventData.eventDuration));
            sequence.Join(this.subtitle.DOFade(0, eventData.eventDuration));
        }
    }

    public void Event_ShowCg(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        bool show = bool.Parse(eventData.eventParam[0]);
        bool autoHide = bool.Parse(eventData.eventParam[1]);
        float duration = float.Parse(eventData.eventParam[2]);
        string resource = eventData.eventParam[3];

        Sprite sprite = Resources.Load<Sprite>(PathManager.CreateCgImagePath(resource));

        cgImg.sprite = sprite;

        int alpha = show == true ? 1 : 0;
        if(show == false) { autoHide = false; }

        float textPos = show == true ? -50f : 0;

        sequence.Append(cgImg.DOFade(alpha, eventData.eventDuration));
        sequence.Join(textMgr.textTransform.DOAnchorPosY(textPos, eventData.eventDuration).SetEase(Ease.OutCubic));

        if (autoHide == true)
        {
            sequence.AppendInterval(duration);

            sequence.Append(cgImg.DOFade(0, eventData.eventDuration));
            sequence.Join(textMgr.textTransform.DOAnchorPosY(0, eventData.eventDuration).SetEase(Ease.OutCubic));
        }
    }

    public void Event_AddLovePoint(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string target = eventData.eventParam[0];
        int amount = int.Parse(eventData.eventParam[1]);

        sequence.AppendCallback(() => GameData.saveFile.AddLovePoint(script, target, amount));
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
    }

    public void Event_Branch(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        List<BranchInfo> branchInfos = BranchInfo.CreateBranchInfo(script);

        int targetScriptId = branchInfos[^1].scriptId;

        for (int i = 0; i < branchInfos.Count; ++i)
        {
            var branchInfo = branchInfos[i];

            if(GameData.saveFile.GetConditionData(branchInfo) >= branchInfo.conditionAmount)
            {
                targetScriptId = branchInfo.scriptId;
                break;
            }
        }

        sequence.AppendCallback(() => Goto(targetScriptId));
    }

    public void RemoveAllChoiceOption(float duration)
    {
        foreach (var btn in choiceOptionList)
        {
            if (btn == null) break;

            Sequence seq = DOTween.Sequence();
            //seq.Append(btn.image.DOFade(0, duration));
            seq.Insert(0, btn.GetComponentInChildren<Text>().DOFade(0, duration));
            seq.AppendCallback(() => Destroy(btn.gameObject));

            seq.Play();
        }
    }

    public UnityEvent<int, int> onChoice = new();
    public UnityEvent<int, int> onChoiceOnce = new();

    public void Event_Choice(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        List<ChoiceInfo> choiceInfos = ChoiceInfo.CreateChoiceInfo(script);

        sequence.AppendCallback(() =>
        {
            for (int i = 0; i < choiceInfos.Count; ++i)
            {
                var choiceInfo = choiceInfos[i];

                ChoiceOptionButton choiceBtn = Instantiate(choiceOptionPref).GetComponent<ChoiceOptionButton>();

                choiceBtn.Init(choiceInfo, () =>
                {
                    foreach(var btn in choiceOptionList)
                    {
                        btn.FadeOut();
                    }

                    choiceOptionList = new();
                });

                choiceBtn.transform.SetParent(choiceOptionParent, false);

                choiceOptionList.Add(choiceBtn);

                choiceBtn.FadeIn(eventData, 0.3f * i);
            }
        });
    }

    public void Event_CloseScenario(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        if (RuntimeData.isEditorMode)
        {
            sequence.AppendCallback(() => {
                DialogManager.Instance.StopDialog();
            });
        }
        else
        {
            sequence.AppendCallback(() => {

                GameData.saveFile.chapterData[RuntimeData.scriptMgr.character] = RuntimeData.scriptMgr.chapter;
                GameData.saveFile.date += 1;

                SaveManager.Save(GameData.saveFile, RuntimeData.saveFileNumber);
            });
            sequence.AppendInterval(1);
            sequence.AppendCallback(() =>
            {
                MysSceneManager.LoadEditorLobbyScene(null);
            });
        }
    }
}
