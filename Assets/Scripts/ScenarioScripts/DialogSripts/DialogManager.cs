using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;

public class DialogManager : Singleton<DialogManager>   
{
    [SerializeField] private EventManager eventMgr;
    [SerializeField] private TextManager textMgr;
    [SerializeField] private MouseEffect mouseEffect;

    [SerializeField] private Button skipBtn;

    public Canvas canvas; //에디터 모드에서 에디터를 숨기기 위해 sortingOrder를 변경함.

    private Sequence autoSkipSeq; //AutoSkip의 경우 시퀀스로 시간 대기 및 스킵을 처리함.
    private IDisposable skipStream; //AutoSkip이 아닌 경우 스킵 입력을 받는 스트림이 생성됨.
    private ScriptObject skipData; //현재 적용되고 있는 스킵 방식을 가진 ScriptObject를 갖고 있도록 함.

    private TweenManager tweenMgr = new();

    public bool isPlaying = false; //현재 대화 씬이 진행 중인지에 대한 여부 (에디터에서 실행/종료를 관리하기 위해 사용)


    #region Events
    public UnityEvent onDialogStart { get; set; } = new();
    public UnityEvent onStart { get; set; } = new();
    public UnityEvent onStop { get; set; } = new();
    public UnityEvent onSkip { get; set; } = new();
    public UnityEvent onNext { get; set; } = new();
    #endregion

    //RuntimeData.scriptMgr에 간략히 접근하는 프로퍼티
    private ScriptManager scriptMgr
    {
        get { return RuntimeData.scriptMgr; }
        set { RuntimeData.scriptMgr = value; }
    }

    /// <summary>
    /// Dialog를 시작하는 함수
    /// </summary>
    /// <param name="scriptGroupId">대화를 진행할 ScriptGroupId</param>
    /// <param name="firstScriptId">시작을 원하는 시점이 있다면 해당 scriptId를 입력</param>
    /// <param name="doNotReset">대화 시작 시 리셋을 원치 않는다면 true로 입력</param>
    public void StartDialog(int scriptGroupId, int firstScriptId = -1, bool doNotReset = false)
    {
        if(doNotReset == false)
        {
            ResetAll(); //모든 대화 관련 변수 및 UI 초기화
        }

        //ScriptManager에 해당 scriptGroupId의 스크립트 불러오기
        scriptMgr = CSVReader.ReadScript(scriptGroupId);

        //firstScriptId가 입력되지 않았다면 해당 스크립트 그룹의 첫 번째 스크립트로 시작하도록 함.
        if(firstScriptId == -1)
        {
            firstScriptId = scriptMgr.firstScript.scriptId;
        }

        scriptMgr.SetCurrentScript(firstScriptId); //firstScriptId로 ScriptManger의 현재 스크립트를 지정함.

        onDialogStart.Invoke();

        isPlaying = true;

        ExecuteScript(scriptMgr.currentScript); //첫 번째 스크립트 호출
    }

    /// <summary>
    /// 스크립트 호출
    /// </summary>
    /// <param name="script">호출할 스크립트</param>
    /// <param name="stopCondition">스크립트 호출을 중단하는 조건 (true 반환 시 중단)</param>
    private void ExecuteScript(in ScriptObject script, Func<ScriptObject, bool> stopCondition = null)
    {
        //stopCondition이 true 반환 시 함수 종료
        if(stopCondition?.Invoke(script) == true) 
        {
            return;
        }

        List<TweenObject> tweenList = CreateTweenList(script, out bool skipException, out Action skipAction);

        PlayScript(tweenList);

        if(skipException == true) //스킵을 진행하지 않는 경우 별도로 반환된 skipAction을 호출
        {
            skipAction?.Invoke();
        }
        else
        {
            SetSkip(script, stopCondition);
        }

        onStart.Invoke();
    }

    /// <summary>
    /// 해당 스크립트와 연결된 모든 스크립트의 TweenObject를 만들고 리스트로 반환
    /// </summary>
    /// <param name="headScript"></param>
    /// <param name="skipException">headScript 또는 연결된 스크립트에 예외적인 스킵 처리가 있다면 true를 반환</param>
    /// <param name="skipAction">skipException이 true라면 별도의 스킵 타입을 반환함</param>
    /// <returns></returns>
    private List<TweenObject> CreateTweenList(in ScriptObject headScript, out bool skipException, out Action skipAction)
    {
        List<TweenObject> tweenList = new();
        ScriptObject script = headScript;

        tweenList.Add(CreateSequence(script)); //headScript로 TweenObject를 만들어 TweenList에 추가

        skipException = false;
        skipAction = null;

        //skipException이 true가 되는 이벤트에 대한 처리
        if (script.scriptType == ScriptType.Event)
        {
            if (script.eventData.eventType == EventType.Choice) //Choice의 경우 선택지를 골라야 다음으로 넘어감. 다음으로 넘어가는 조건 역시 선택지를 고를 때이므로 skipAction이 없음.
            {
                skipException = true;
                skipAction = null;
            }
        }

        //연결된 모든 스크립트 순회
        while (script.linkEvent == true && scriptMgr.nextScript != null && scriptMgr.nextScript.scriptType != ScriptType.Text)
        {
            script = scriptMgr.Next();

            tweenList.Add(CreateSequence(script)); //해당 스크립트로 TweenObject를 만들어 TweenList에 추가

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

    /// <summary>
    /// 스크립트로 TweenObject를 만들어 반환함
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    private TweenObject CreateSequence(in ScriptObject script)
    {
        Sequence tween;

        //스크립트 타입에 맞는 함수를 호출하여 시퀀스 생성.
        if (script.scriptType == ScriptType.Event)
        {
            tween = eventMgr.CreateEventSequence(script);
        }
        else
        {
            tween = textMgr.CreateTextSequence(script);
        }

        //만들어진 시퀀스로 TweenObject 생성
        TweenObject tweenObj = new(tween, script, tweenMgr);

        //TweenObject의 turn 관련 변수 설정
        if (script.eventData.durationTurn > 0)
        {
            tweenObj.durationTurn = script.eventData.durationTurn;
            tweenObj.remainingTurn = script.eventData.durationTurn;
        }

        return tweenObj;
    }

    /// <summary>
    /// tweenList를 tweenManager에 추가하고 시작함.
    /// </summary>
    /// <param name="tweenList"></param>
    private void PlayScript(in List<TweenObject> tweenList)
    {
        tweenMgr.AddRange(tweenList);

        tweenMgr.PlayAllTweens();
    }

    #region Skip
    /// <summary>
    /// SkipMethod에 따라 스킵을 처리함.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="stopCondition"></param>
    private void SetSkip(ScriptObject script, Func<ScriptObject, bool> stopCondition = null)
    {
        skipData = script;

        if (script.skipMethod == SkipMethod.Auto) //AutoSkip일 경우 가장 긴 트윈이 종료된 이후 Next가 호출되도록 함.
        {
            Sequence skipSeq = DOTween.Sequence();
            autoSkipSeq = skipSeq;

            float duration = tweenMgr.FindLongestDuration();

            float skipInterval = script.skipDelay;
            if (duration != -1) //duration이 -1인 경우는 무한 루프 시퀀스만 남은 상태를 의미.
            {
                skipInterval += duration;
            }

            skipSeq.AppendInterval(skipInterval);
            skipSeq.AppendCallback(() => Next(stopCondition));

            skipSeq.Play();
        }
        else if (script.skipMethod == SkipMethod.Skipable || script.skipMethod == SkipMethod.NoSkip)
        {
            //가장 긴 트윈이 종료된 경우에 MouseEffect를 액티브하는 스트림 생성 (스킵 입력 없이 트윈이 종료되는 경우를 위해 필요)
            Observable.Timer(TimeSpan.FromSeconds(tweenMgr.FindLongestDuration()))
                .Subscribe(_ => mouseEffect.ActiveMouseEffect(true && !string.IsNullOrWhiteSpace(textMgr.text.text), textMgr.GetLastLineTextLength()));

            //스페이스가 입력되면 스킵을 실행하는 스트림 생성
            skipStream = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => Skip(script, stopCondition));

            //스킵 버튼 클릭 시에도 스킵 실행
            skipBtn.onClick.AddListener(() => Skip(script, stopCondition));
        }
    }

    /// <summary>
    /// 현재 진행중인 트윈의 Complete를 호출하거나 다음 스크립트를 호출함.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="stopCondition"></param>
    private void Skip(in ScriptObject script, Func<ScriptObject, bool> stopCondition = null)
    {
        bool isPlaying = tweenMgr.ExistPlayingTween();

        if (script.skipMethod == SkipMethod.Skipable && isPlaying == true) //Skipable 타입인데 재생중인 트윈이 있다면 스킵해버리기.
        {
            tweenMgr.SkipAllTweens();

            onSkip.Invoke();

            mouseEffect.ActiveMouseEffect(true && !string.IsNullOrWhiteSpace(textMgr.text.text), textMgr.GetLastLineTextLength());
        }
        else if (isPlaying == false) //스킵 타입과 관계없이 isPlaying이 false라면 다음 스크립트로 이동
        {
            Next(stopCondition);
        }
    }
    #endregion

    /// <summary>
    /// 다음 스크립트를 호출합니다.
    /// </summary>
    /// <param name="stopCondition"></param>
    private void Next(Func<ScriptObject, bool> stopCondition = null)
    {
        tweenMgr.SkipAllTweens();

        skipStream?.Dispose();

        skipBtn.onClick.RemoveAllListeners();

        mouseEffect.ActiveMouseEffect(false, textMgr.GetLastLineTextLength());

        if (scriptMgr.nextScript != null) //다음 스크립트가 존재한다면 다음 스크립트 호출
        {
            ScriptObject nextScript = scriptMgr.Next();

            ExecuteScript(nextScript, stopCondition);

            onNext.Invoke();
        }
        else //다음 스크립트가 종료되었다면 대화 종료. (스크립트 종료는 이벤트를 통해 이루어지기에 이 경우는 비정상적인 종료임.)
        {
            "모든 스크립트가 종료되었습니다.".LogError();
            StopDialog();
        }
    }

    /// <summary>
    /// 대화 플로우를 중지합니다.
    /// </summary>
    public void StopDialog()
    {
        tweenMgr.StopAllTweens();

        skipStream?.Dispose();

        skipBtn.onClick.RemoveAllListeners();

        isPlaying = false;

        onStop?.Invoke();
    }

    /// <summary>
    /// 특정 스크립트의 Id로 즉시 이동합니다. 현재 스크립트와 해당 스크립트 사이의 스크립트는 무시됩니다.
    /// </summary>
    /// <param name="scriptId"></param>
    public void Goto(int scriptId)
    {
        StopDialog();

        int groupId = ScriptManager.GetScriptGroupId(scriptId);

        StartDialog(groupId, scriptId);
    }

    /// <summary>
    /// 모든 대화 관련 변수 및 UI 초기화
    /// </summary>
    public void ResetAll()
    {
        eventMgr.ResetAll();
        textMgr.ResetAll();
        mouseEffect.ActiveMouseEffect(false, textMgr.GetLastLineTextLength());
    }

    /// <summary>
    /// 스크립트 그룹의 특정 스크립트까지 즉시 진행합니다. 사이의 모든 스크립트가 실행되고 스킵됩니다.<br></br>
    /// 에디터를 위해서만 사용됩니다.
    /// </summary>
    /// <param name="scriptGroupId"></param>
    /// <param name="targetScriptId"></param>
    public void MoveTo(int scriptGroupId, int targetScriptId)
    {
        ResetAll();

        scriptMgr = CSVReader.ReadScript(scriptGroupId);

        foreach (var script in scriptMgr.scripts) //스크립트의 텍스트, 이벤트가 0초만에 종료되도록 함.
        {
            script.textDuration = 0;
            script.eventData.eventDuration = 0;
            script.eventData.eventDelay = 0;
            script.skipMethod = SkipMethod.Auto;
            script.skipDelay = 0;
        }

        int firstScriptId = scriptMgr.firstScript.scriptId;

        scriptMgr.SetCurrentScript(firstScriptId);

        onDialogStart.Invoke();

        isPlaying = true;

        //스크립트를 호출하되, targetScript가 되면 종료되도록 stopConditon을 전달.
        //대화 진행 플로우의 모든 stopConditon 매개변수는 여기서 전달한 stopCondition을 계속해서 전달하기 위함임.
        ExecuteScript(scriptMgr.currentScript, (script) =>
        {
            if(script.scriptId == targetScriptId) //현재 ScriptId가 targetScriptId라면 중지하고 대화를 시작하도록 함.
            {
                StartDialog(scriptGroupId, targetScriptId, true);
                return true;
            }

            return false;
        });
    }
}
