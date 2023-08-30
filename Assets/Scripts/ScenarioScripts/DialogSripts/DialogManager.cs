using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UniRx;
using System.IO.Compression;

public class DialogManager : Singleton<DialogManager>   
{
    [SerializeField] private EventManager eventMgr;
    [SerializeField] private TextManager textMgr;
    public Canvas canvas;

    private Sequence autoSkipSeq;
    private IDisposable skipStream;

    private TweenManager tweenMgr = new();

    public bool isPlaying = false;


    #region Events
    public UnityEvent onDialogStart { get; set; } = new();
    public UnityEvent onStart { get; set; } = new();
    public UnityEvent onStop { get; set; } = new();
    public UnityEvent onSkip { get; set; } = new();
    public UnityEvent onNext { get; set; } = new();
    #endregion

    private ScriptManager scriptMgr
    {
        get { return RuntimeData.scriptMgr; }
        set { RuntimeData.scriptMgr = value; }
    }

    private void Start()
    {
        //Observable.EveryUpdate()
        //    .Where(_ => Input.GetKeyDown(KeyCode.S))
        //    .Subscribe(_ => MysSceneManager.LoadLobbyScene(null));
    }

    public void StartDialog(int scriptGroupId, int firstScriptId = -1)
    {
        ResetAll();

        scriptMgr = CSVReader.ReadScript(scriptGroupId);

        if(firstScriptId == -1)
        {
            firstScriptId = scriptMgr.firstScript.scriptId;
        }

        scriptMgr.SetCurrentScript(firstScriptId);

        onDialogStart.Invoke();

        isPlaying = true;

        ExecuteScript(scriptMgr.currentScript);
    }


    private void ExecuteScript(in ScriptObject script)
    {
        List<TweenObject> tweenList = CreateTweenList(script, out bool skipException, out Action skipAction);

        PlayScript(tweenList);

        if(skipException == true)
        {
            skipAction?.Invoke();
        }
        else
        {
            SetSkip(script);
        }

        onStart.Invoke();
    }

    private List<TweenObject> CreateTweenList(in ScriptObject headScript, out bool skipException, out Action skipAction)
    {
        List<TweenObject> tweenList = new();
        ScriptObject script = headScript;

        tweenList.Add(CreateSequence(script));

        skipException = false;
        skipAction = null;

        if (script.scriptType == ScriptType.Event)
        {
            if (script.eventData.eventType == EventType.Choice)
            {
                skipException = true;
                skipAction = null;
            }
        }

        while (script.linkEvent == true && scriptMgr.nextScript != null && scriptMgr.nextScript.scriptType != ScriptType.Text)
        {
            script = scriptMgr.Next();

            tweenList.Add(CreateSequence(script));

            //별도의 스킵 방식을 사용하는 이벤트 처리
            if(script.scriptType == ScriptType.Event)
            {
                if(script.eventData.eventType == EventType.Choice)
                {
                    skipException = true;
                    skipAction = null;
                }
            }
        }

        return tweenList;
    }

    private TweenObject CreateSequence(in ScriptObject script)
    {
        Sequence tween;

        if (script.scriptType == ScriptType.Event)
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

    private void PlayScript(in List<TweenObject> tweenList)
    {
        tweenMgr.AddRange(tweenList);

        tweenMgr.PlayAllTweens();
    }

    #region Skip
    private void SetSkip(ScriptObject script)
    {
        if (script.skipMethod == SkipMethod.Auto) //Auto 스킵일 경우 가장 긴 트윈이 종료된 이후 Next가 호출되도록 함.
        {
            Sequence skipSeq = DOTween.Sequence();
            autoSkipSeq = skipSeq;

            float duration = tweenMgr.FindLongestDuration();

            float skipInterval = script.skipDelay;
            if (duration != -1)
            {
                skipInterval += duration;
            }

            skipSeq.AppendInterval(skipInterval);
            skipSeq.AppendCallback(Next);

            skipSeq.Play();
        }
        else if (script.skipMethod == SkipMethod.Skipable || script.skipMethod == SkipMethod.NoSkip)
        {
            skipStream = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => Skip(script));
        }
    }

    private void Skip(in ScriptObject script)
    {
        bool isPlaying = tweenMgr.ExistPlayingTween();

        if (script.skipMethod == SkipMethod.Skipable && isPlaying == true) //Skipable 타입인데 재생중인 트윈이 있다면 스킵해버리기.
        {
            tweenMgr.SkipAllTweens();

            onSkip.Invoke();
        }
        else if (isPlaying == false) //스킵 타입과 관계없이 isPlaying이 false라면 다음 스크립트로 이동
        {
            Next();
        }
    }
    #endregion

    private void Next()
    {
        tweenMgr.SkipAllTweens();

        skipStream?.Dispose();

        if(scriptMgr.nextScript != null)
        {
            ScriptObject nextScript = scriptMgr.Next();

            ExecuteScript(nextScript);

            onNext.Invoke();
        }
        else
        {
            "모든 스크립트가 종료되었습니다.".LogError();
            StopDialog();
        }
    }

    public void StopDialog()
    {
        tweenMgr.StopAllTweens();

        skipStream?.Dispose();

        isPlaying = false;

        onStop?.Invoke();
    }

    public void Goto(int scriptId)
    {
        StopDialog();

        int groupId = ScriptManager.GetScriptGroupId(scriptId);

        StartDialog(groupId, scriptId);
    }

    public void ResetAll()
    {
        eventMgr.ResetAll();
    }
}
