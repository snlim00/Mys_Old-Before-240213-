using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Awake()
    {
        textMgr = FindObjectOfType<NewTextManager>();
        eventMgr = FindObjectOfType<NewEventManager>();
    }

    private void Start()
    {
        DialogStart(2);
    }

    public void DialogStart(int scriptGroupID, int firstScriptID = -1)
    {
        scriptMgr.ReadScript(scriptGroupID);

        if(firstScriptID == -1) 
        { 
            firstScriptID = NewScriptManager.GetFirstScriptIDFromGroupID(scriptGroupID);
        }
        scriptMgr.SetCurrentScript(firstScriptID);

        ExecuteScript(scriptMgr.currentScript);
    }

    #region Execute Script
    private void ExecuteScript(in ScriptObject script)
    {
        List<TweenObject> tweenList = CreateTweenList(script);

        tweenMgr.AddRange(tweenList);

        tweenMgr.PlayAllTweens();

        SetSkip(script);
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
        }
        else
        {
            "모든 스크립트가 종료되었습니다.".LogError("Next", true);
        }
    }
}
