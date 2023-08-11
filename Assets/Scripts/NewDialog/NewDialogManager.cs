using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UniRx;

public class NewDialogManager : Singleton<NewDialogManager>
{
    private Sequence autoSkipSequence = null;
    private IDisposable skipStream = null;

    private NewTextManager textMgr;
    private NewEventManager eventMgr;

    public TweenManager tweenMgr = new();

    public NewScriptManager scriptMgr;

    public UnityEvent onDialogStart { get; set; } = new();
    public UnityEvent onStart { get; set; } = new();
    public UnityEvent onSkip { get; set; } = new();
    public UnityEvent onNext { get; set; } = new();

    private void Awake()
    {
        textMgr = FindObjectOfType<NewTextManager>();
        eventMgr = FindObjectOfType<NewEventManager>();
    }

    private void Start()
    {
        //DialogStart(2);
        //ExecuteMoveTo(20004);
    }

    public void DialogStart(int scriptGroupId, int firstScriptId = -1)
    {
        scriptMgr = CSVReader.ReadScript(scriptGroupId);

        scriptMgr.chapter.Log();
        scriptMgr.title.Log();
        scriptMgr.explain.Log();

        if(firstScriptId == -1) 
        { 
            firstScriptId = scriptMgr.firstScript.scriptId;
        }
        scriptMgr.SetCurrentScript(firstScriptId);

        ExecuteScript(scriptMgr.currentScript);

        onDialogStart.Invoke();
    }

    #region Execute Script
    private void ExecuteScript(in ScriptObject script)
    {
        List<TweenObject> tweenList = CreateTweenList(script);

        PlayScript(tweenList);

        bool isChoice = false;
        tweenMgr.DoAllTweens(tweenObj =>
        {
            ScriptObject script = tweenObj.script;
            
            if(script.scriptType == ScriptType.Event && script.eventData.eventType == EventType.Choice)
            {
                isChoice = true;
                return;
            }
        });
        if (isChoice == true) return;

        SetSkip(script);

        onStart.Invoke();
    }

    public void PlayScript(in List<TweenObject> tweenList)
    {
        tweenMgr.AddRange(tweenList);

        tweenMgr.PlayAllTweens();
    }

    private List<TweenObject> CreateTweenList(in ScriptObject firstScript)
    {
        List<TweenObject> tweenList = new();
        ScriptObject script = firstScript;

        tweenList.Add(CreateSequence(script));

        while (script.linkEvent == true && scriptMgr.nextScript != null && scriptMgr.nextScript.scriptType != ScriptType.Text)
        {
            script = scriptMgr.Next();

            tweenList.Add(CreateSequence(script));
        }

        return tweenList;
    }

    private TweenObject CreateSequence(in ScriptObject script)
    {
        Sequence tween;

        if(script.scriptType == ScriptType.Event)
        {
            tween = eventMgr.CreateEventSequence(script);
        }
        else
        {
            tween = textMgr.CreateTextSequence(script);
        }

        TweenObject tweenObj = new(tween, script, tweenMgr);

        if (script.eventData.durationTurn > 0)
        {
            tweenObj.durationTurn = script.eventData.durationTurn;
            tweenObj.remainingTurn = script.eventData.durationTurn;
        }

        return tweenObj;
    }
    #endregion

    #region Skip
    private void SetSkip(ScriptObject script)
    {
        if(script.skipMethod == SkipMethod.Auto) //Auto ��ŵ�� ��� ���� �� Ʈ���� ����� ���� Next�� ȣ��ǵ��� ��.
        {
            Sequence skipSeq = DOTween.Sequence();
            autoSkipSequence = skipSeq;

            float duration = tweenMgr.FindLongestDuration();

            float skipInterval = script.skipDelay;
            if(duration != -1)
            {
                skipInterval += duration;
            }

            skipSeq.AppendInterval(skipInterval);
            skipSeq.AppendCallback(Next);

            skipSeq.Play();
        }
        else if(script.skipMethod == SkipMethod.Skipable || script.skipMethod == SkipMethod.NoSkip)
        {
            skipStream = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => Skip(script));
        }
    }

    private void Skip(in ScriptObject script)
    {
        bool isPlaying = tweenMgr.ExistPlayingTween();

        if(script.skipMethod == SkipMethod.Skipable && isPlaying == true) //Skipable Ÿ���ε� ������� Ʈ���� �ִٸ� ��ŵ�ع�����.
        {
            tweenMgr.SkipAllTweens();

            onSkip.Invoke();
        }
        else if(isPlaying == false) //��ŵ Ÿ�԰� ������� isPlaying�� false��� ���� ��ũ��Ʈ�� �̵�
        {
            Next();
        }
    }
    #endregion

    private void Next()
    {
        tweenMgr.SkipAllTweens();

        skipStream?.Dispose(); //skipStream �ߴܽ�Ű��

        if(scriptMgr.nextScript != null)
        {
            ExecuteScript(scriptMgr.Next());
            //ExecuteScript(scriptMgr.currentScript);
            onNext.Invoke();
        }
        else
        {
            "��� ��ũ��Ʈ�� ����Ǿ����ϴ�.".LogError("Next", true);
        }
    }

    #region Stop
    public void StopDialog()
    {
        tweenMgr.StopAllTweens();

        skipStream?.Dispose();
    }
    #endregion

    public void ResetAll()
    {
        "ResetAll".Log();
        eventMgr.RemoveAllChoiceOption(0);

        eventMgr.SetBackground(null);
        eventMgr.RemoveAllObject();

        textMgr.textBox.SetAlpha(1);
        textMgr.text.SetAlpha(1);
        textMgr.characterName.SetAlpha(1);
    }

    #region Goto / Moveto
    public void Goto(int scriptId)
    {
        StopDialog();

        DialogStart(NewScriptManager.GetScriptGroupId(scriptId), scriptId);
    }

    public void ExecuteMoveTo(int targetId)
    {
        ResetAll();

        int groupId = scriptMgr.scriptGroupId;

        ScriptObject firstScript = scriptMgr.firstScript;

        scriptMgr.SetCurrentScript(firstScript.scriptId);

        MoveTo(firstScript, targetId);
    }

    private void MoveTo(ScriptObject currentScript, int targetId)
    {
        if(currentScript.scriptId >= targetId)
        {
            DialogStart(NewScriptManager.GetScriptGroupId(currentScript.scriptId), currentScript.scriptId);
            return;
        }

        List<TweenObject> tweenList = CreateTweenList(currentScript);
        PlayScript(tweenList);

        bool isChoice = false;
        tweenMgr.DoAllTweens(tweenObj =>
        {
            ScriptObject script = tweenObj.script;

            if (script.scriptType == ScriptType.Event && script.eventData.eventType == EventType.Choice)
            {
                isChoice = true;
                script.scriptId.Log();
                return;
            }
        });

        if (isChoice == true)
        {

            "IS CHOICE IS TRUE".Log();
            //���Ŀ� � �������� ���� �ش� ��ũ��Ʈ���� targetID���� ����Ǵ� ExecuteMoveTo�� ȣ���ϵ��� �ؾ� ��.
            //�ش� ������ ���� ���Ŀ� �ؽ�Ʈ �������� ���ÿ� ����Ǵ� ���� ����.
                //Goto�� �̵��ϰ� �Ǵ� ������ �ؽ�Ʈ�� �߻��ϴ� ������ ������. (�ش� �ؽ�Ʈ�� moveTO�Լ� ������ ������� ����)
                    //������ ������ ����̴� �������� ����? 230427
            eventMgr.onChoiceOnce.AddListener((index, scriptId) =>
            {
                var script = scriptMgr.GetScript(scriptId);
                scriptMgr.SetCurrentScript(script.scriptId);
                MoveTo(script, targetId);
                targetId.Log("ON CHOICE ONCE");
            });

            return;
        }

        tweenMgr.StopAllTweens();

        MoveTo(scriptMgr.Next(), targetId);
    }
    #endregion
}
