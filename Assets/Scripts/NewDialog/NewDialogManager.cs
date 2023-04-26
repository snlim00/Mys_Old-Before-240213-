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

    public NewScriptManager scriptMgr = new();

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
        ExecuteMoveTo(20004);
    }

    public void DialogStart(int scriptGroupID, int firstScriptID = -1)
    {
        firstScriptID.Log("Dialog Start");

        scriptMgr.ReadScript(scriptGroupID);

        if(firstScriptID == -1) 
        { 
            firstScriptID = NewScriptManager.GetFirstScriptIDFromGroupID(scriptGroupID);
        }
        scriptMgr.SetCurrentScript(firstScriptID);

        ExecuteScript(scriptMgr.currentScript);

        onDialogStart.Invoke();
    }

    #region Execute Script
    private void ExecuteScript(in ScriptObject script)
    {
        List<TweenObject> tweenList = CreateTweenList(script);

        tweenMgr.AddRange(tweenList);

        tweenMgr.PlayAllTweens();

        SetSkip(script);

        onStart.Invoke();
    }

    private List<TweenObject> CreateTweenList(in ScriptObject firstScript)
    {
        List<TweenObject> tweenList = new();
        ScriptObject script = firstScript;

        tweenList.Add(CreateSequence(script));

        while (script.linkEvent == true || script.scriptType != ScriptType.Text)
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
        if(script.skipMethod == SkipMethod.Auto) //Auto 스킵일 경우 가장 긴 트윈이 종료된 이후 Next가 호출되도록 함.
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

        if(script.skipMethod == SkipMethod.Skipable && isPlaying == true) //Skipable 타입인데 재생중인 트윈이 있다면 스킵해버리기.
        {
            tweenMgr.SkipAllTweens();

            onSkip.Invoke();
        }
        else if(isPlaying == false) //스킵 타입과 관계없이 isPlaying이 false라면 다음 스크립트로 이동
        {
            Next();
        }
    }
    #endregion

    private void Next()
    {
        tweenMgr.SkipAllTweens();

        skipStream?.Dispose(); //skipStream 중단시키기

        if(scriptMgr.GetNextScript() != null)
        {
            ExecuteScript(scriptMgr.Next());
            //ExecuteScript(scriptMgr.currentScript);
            onNext.Invoke();
        }
        else
        {
            "모든 스크립트가 종료되었습니다.".LogError("Next", true);
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
        eventMgr.RemoveAllChoiceOption(0);

        eventMgr.SetBackground(null);
        eventMgr.RemoveAllObject();

        textMgr.textBox.SetAlpha(1);
        textMgr.text.SetAlpha(1);
        textMgr.characterName.SetAlpha(1);
    }

    #region Goto / Moveto
    public void Goto(int scriptID)
    {
        StopDialog();

        DialogStart(scriptID);
    }

    public void ExecuteMoveTo(int targetID)
    {
        ResetAll();
        
        scriptMgr.ReadScript(ScriptManager.GetGroupID(targetID));

        ScriptObject firstScript = scriptMgr.GetScript(ScriptManager.GetFirstScriptIDFromScriptID(targetID));

        scriptMgr.SetCurrentScript(firstScript.scriptID);

        MoveTo(firstScript, targetID);
    }

    private void MoveTo(ScriptObject currentScript, int targetID)
    {
        if(currentScript.scriptID >= targetID)
        {
            DialogStart(ScriptManager.GetGroupID(currentScript.scriptID), currentScript.scriptID);
            return;
        }

        List<TweenObject> tweenList = CreateTweenList(currentScript);
        tweenMgr.AddRange(tweenList);

        //tweenMgr.DoAllTweensForModify(tweenObj =>
        //{
        //    if(tweenObj.script.scriptType == ScriptType.Text)
        //    {
        //        tweenObj.script.Log();
        //        tweenMgr.RemoveTween(tweenObj);
        //    }
        //});

        tweenMgr.PlayAllTweens();
        tweenMgr.StopAllTweens();

        MoveTo(scriptMgr.Next(), targetID);
    }
    #endregion
}
