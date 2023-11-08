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

    public Canvas canvas; //������ ��忡�� �����͸� ����� ���� sortingOrder�� ������.

    private Sequence autoSkipSeq; //AutoSkip�� ��� �������� �ð� ��� �� ��ŵ�� ó����.
    private IDisposable skipStream; //AutoSkip�� �ƴ� ��� ��ŵ �Է��� �޴� ��Ʈ���� ������.
    private ScriptObject skipData; //���� ����ǰ� �ִ� ��ŵ ����� ���� ScriptObject�� ���� �ֵ��� ��.

    private TweenManager tweenMgr = new();

    public bool isPlaying = false; //���� ��ȭ ���� ���� �������� ���� ���� (�����Ϳ��� ����/���Ḧ �����ϱ� ���� ���)


    #region Events
    public UnityEvent onDialogStart { get; set; } = new();
    public UnityEvent onStart { get; set; } = new();
    public UnityEvent onStop { get; set; } = new();
    public UnityEvent onSkip { get; set; } = new();
    public UnityEvent onNext { get; set; } = new();
    #endregion

    //RuntimeData.scriptMgr�� ������ �����ϴ� ������Ƽ
    private ScriptManager scriptMgr
    {
        get { return RuntimeData.scriptMgr; }
        set { RuntimeData.scriptMgr = value; }
    }

    /// <summary>
    /// Dialog�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="scriptGroupId">��ȭ�� ������ ScriptGroupId</param>
    /// <param name="firstScriptId">������ ���ϴ� ������ �ִٸ� �ش� scriptId�� �Է�</param>
    /// <param name="doNotReset">��ȭ ���� �� ������ ��ġ �ʴ´ٸ� true�� �Է�</param>
    public void StartDialog(int scriptGroupId, int firstScriptId = -1, bool doNotReset = false)
    {
        if(doNotReset == false)
        {
            ResetAll(); //��� ��ȭ ���� ���� �� UI �ʱ�ȭ
        }

        //ScriptManager�� �ش� scriptGroupId�� ��ũ��Ʈ �ҷ�����
        scriptMgr = CSVReader.ReadScript(scriptGroupId);

        //firstScriptId�� �Էµ��� �ʾҴٸ� �ش� ��ũ��Ʈ �׷��� ù ��° ��ũ��Ʈ�� �����ϵ��� ��.
        if(firstScriptId == -1)
        {
            firstScriptId = scriptMgr.firstScript.scriptId;
        }

        scriptMgr.SetCurrentScript(firstScriptId); //firstScriptId�� ScriptManger�� ���� ��ũ��Ʈ�� ������.

        onDialogStart.Invoke();

        isPlaying = true;

        ExecuteScript(scriptMgr.currentScript); //ù ��° ��ũ��Ʈ ȣ��
    }

    /// <summary>
    /// ��ũ��Ʈ ȣ��
    /// </summary>
    /// <param name="script">ȣ���� ��ũ��Ʈ</param>
    /// <param name="stopCondition">��ũ��Ʈ ȣ���� �ߴ��ϴ� ���� (true ��ȯ �� �ߴ�)</param>
    private void ExecuteScript(in ScriptObject script, Func<ScriptObject, bool> stopCondition = null)
    {
        //stopCondition�� true ��ȯ �� �Լ� ����
        if(stopCondition?.Invoke(script) == true) 
        {
            return;
        }

        List<TweenObject> tweenList = CreateTweenList(script, out bool skipException, out Action skipAction);

        PlayScript(tweenList);

        if(skipException == true) //��ŵ�� �������� �ʴ� ��� ������ ��ȯ�� skipAction�� ȣ��
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
    /// �ش� ��ũ��Ʈ�� ����� ��� ��ũ��Ʈ�� TweenObject�� ����� ����Ʈ�� ��ȯ
    /// </summary>
    /// <param name="headScript"></param>
    /// <param name="skipException">headScript �Ǵ� ����� ��ũ��Ʈ�� �������� ��ŵ ó���� �ִٸ� true�� ��ȯ</param>
    /// <param name="skipAction">skipException�� true��� ������ ��ŵ Ÿ���� ��ȯ��</param>
    /// <returns></returns>
    private List<TweenObject> CreateTweenList(in ScriptObject headScript, out bool skipException, out Action skipAction)
    {
        List<TweenObject> tweenList = new();
        ScriptObject script = headScript;

        tweenList.Add(CreateSequence(script)); //headScript�� TweenObject�� ����� TweenList�� �߰�

        skipException = false;
        skipAction = null;

        //skipException�� true�� �Ǵ� �̺�Ʈ�� ���� ó��
        if (script.scriptType == ScriptType.Event)
        {
            if (script.eventData.eventType == EventType.Choice) //Choice�� ��� �������� ���� �������� �Ѿ. �������� �Ѿ�� ���� ���� �������� �� ���̹Ƿ� skipAction�� ����.
            {
                skipException = true;
                skipAction = null;
            }
        }

        //����� ��� ��ũ��Ʈ ��ȸ
        while (script.linkEvent == true && scriptMgr.nextScript != null && scriptMgr.nextScript.scriptType != ScriptType.Text)
        {
            script = scriptMgr.Next();

            tweenList.Add(CreateSequence(script)); //�ش� ��ũ��Ʈ�� TweenObject�� ����� TweenList�� �߰�

            //������ ��ŵ ����� ����ϴ� �̺�Ʈ ó��
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
    /// ��ũ��Ʈ�� TweenObject�� ����� ��ȯ��
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    private TweenObject CreateSequence(in ScriptObject script)
    {
        Sequence tween;

        //��ũ��Ʈ Ÿ�Կ� �´� �Լ��� ȣ���Ͽ� ������ ����.
        if (script.scriptType == ScriptType.Event)
        {
            tween = eventMgr.CreateEventSequence(script);
        }
        else
        {
            tween = textMgr.CreateTextSequence(script);
        }

        //������� �������� TweenObject ����
        TweenObject tweenObj = new(tween, script, tweenMgr);

        //TweenObject�� turn ���� ���� ����
        if (script.eventData.durationTurn > 0)
        {
            tweenObj.durationTurn = script.eventData.durationTurn;
            tweenObj.remainingTurn = script.eventData.durationTurn;
        }

        return tweenObj;
    }

    /// <summary>
    /// tweenList�� tweenManager�� �߰��ϰ� ������.
    /// </summary>
    /// <param name="tweenList"></param>
    private void PlayScript(in List<TweenObject> tweenList)
    {
        tweenMgr.AddRange(tweenList);

        tweenMgr.PlayAllTweens();
    }

    #region Skip
    /// <summary>
    /// SkipMethod�� ���� ��ŵ�� ó����.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="stopCondition"></param>
    private void SetSkip(ScriptObject script, Func<ScriptObject, bool> stopCondition = null)
    {
        skipData = script;

        if (script.skipMethod == SkipMethod.Auto) //AutoSkip�� ��� ���� �� Ʈ���� ����� ���� Next�� ȣ��ǵ��� ��.
        {
            Sequence skipSeq = DOTween.Sequence();
            autoSkipSeq = skipSeq;

            float duration = tweenMgr.FindLongestDuration();

            float skipInterval = script.skipDelay;
            if (duration != -1) //duration�� -1�� ���� ���� ���� �������� ���� ���¸� �ǹ�.
            {
                skipInterval += duration;
            }

            skipSeq.AppendInterval(skipInterval);
            skipSeq.AppendCallback(() => Next(stopCondition));

            skipSeq.Play();
        }
        else if (script.skipMethod == SkipMethod.Skipable || script.skipMethod == SkipMethod.NoSkip)
        {
            //���� �� Ʈ���� ����� ��쿡 MouseEffect�� ��Ƽ���ϴ� ��Ʈ�� ���� (��ŵ �Է� ���� Ʈ���� ����Ǵ� ��츦 ���� �ʿ�)
            Observable.Timer(TimeSpan.FromSeconds(tweenMgr.FindLongestDuration()))
                .Subscribe(_ => mouseEffect.ActiveMouseEffect(true && !string.IsNullOrWhiteSpace(textMgr.text.text), textMgr.GetLastLineTextLength()));

            //�����̽��� �ԷµǸ� ��ŵ�� �����ϴ� ��Ʈ�� ����
            skipStream = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => Skip(script, stopCondition));

            //��ŵ ��ư Ŭ�� �ÿ��� ��ŵ ����
            skipBtn.onClick.AddListener(() => Skip(script, stopCondition));
        }
    }

    /// <summary>
    /// ���� �������� Ʈ���� Complete�� ȣ���ϰų� ���� ��ũ��Ʈ�� ȣ����.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="stopCondition"></param>
    private void Skip(in ScriptObject script, Func<ScriptObject, bool> stopCondition = null)
    {
        bool isPlaying = tweenMgr.ExistPlayingTween();

        if (script.skipMethod == SkipMethod.Skipable && isPlaying == true) //Skipable Ÿ���ε� ������� Ʈ���� �ִٸ� ��ŵ�ع�����.
        {
            tweenMgr.SkipAllTweens();

            onSkip.Invoke();

            mouseEffect.ActiveMouseEffect(true && !string.IsNullOrWhiteSpace(textMgr.text.text), textMgr.GetLastLineTextLength());
        }
        else if (isPlaying == false) //��ŵ Ÿ�԰� ������� isPlaying�� false��� ���� ��ũ��Ʈ�� �̵�
        {
            Next(stopCondition);
        }
    }
    #endregion

    /// <summary>
    /// ���� ��ũ��Ʈ�� ȣ���մϴ�.
    /// </summary>
    /// <param name="stopCondition"></param>
    private void Next(Func<ScriptObject, bool> stopCondition = null)
    {
        tweenMgr.SkipAllTweens();

        skipStream?.Dispose();

        skipBtn.onClick.RemoveAllListeners();

        mouseEffect.ActiveMouseEffect(false, textMgr.GetLastLineTextLength());

        if (scriptMgr.nextScript != null) //���� ��ũ��Ʈ�� �����Ѵٸ� ���� ��ũ��Ʈ ȣ��
        {
            ScriptObject nextScript = scriptMgr.Next();

            ExecuteScript(nextScript, stopCondition);

            onNext.Invoke();
        }
        else //���� ��ũ��Ʈ�� ����Ǿ��ٸ� ��ȭ ����. (��ũ��Ʈ ����� �̺�Ʈ�� ���� �̷�����⿡ �� ���� ���������� ������.)
        {
            "��� ��ũ��Ʈ�� ����Ǿ����ϴ�.".LogError();
            StopDialog();
        }
    }

    /// <summary>
    /// ��ȭ �÷ο츦 �����մϴ�.
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
    /// Ư�� ��ũ��Ʈ�� Id�� ��� �̵��մϴ�. ���� ��ũ��Ʈ�� �ش� ��ũ��Ʈ ������ ��ũ��Ʈ�� ���õ˴ϴ�.
    /// </summary>
    /// <param name="scriptId"></param>
    public void Goto(int scriptId)
    {
        StopDialog();

        int groupId = ScriptManager.GetScriptGroupId(scriptId);

        StartDialog(groupId, scriptId);
    }

    /// <summary>
    /// ��� ��ȭ ���� ���� �� UI �ʱ�ȭ
    /// </summary>
    public void ResetAll()
    {
        eventMgr.ResetAll();
        textMgr.ResetAll();
        mouseEffect.ActiveMouseEffect(false, textMgr.GetLastLineTextLength());
    }

    /// <summary>
    /// ��ũ��Ʈ �׷��� Ư�� ��ũ��Ʈ���� ��� �����մϴ�. ������ ��� ��ũ��Ʈ�� ����ǰ� ��ŵ�˴ϴ�.<br></br>
    /// �����͸� ���ؼ��� ���˴ϴ�.
    /// </summary>
    /// <param name="scriptGroupId"></param>
    /// <param name="targetScriptId"></param>
    public void MoveTo(int scriptGroupId, int targetScriptId)
    {
        ResetAll();

        scriptMgr = CSVReader.ReadScript(scriptGroupId);

        foreach (var script in scriptMgr.scripts) //��ũ��Ʈ�� �ؽ�Ʈ, �̺�Ʈ�� 0�ʸ��� ����ǵ��� ��.
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

        //��ũ��Ʈ�� ȣ���ϵ�, targetScript�� �Ǹ� ����ǵ��� stopConditon�� ����.
        //��ȭ ���� �÷ο��� ��� stopConditon �Ű������� ���⼭ ������ stopCondition�� ����ؼ� �����ϱ� ������.
        ExecuteScript(scriptMgr.currentScript, (script) =>
        {
            if(script.scriptId == targetScriptId) //���� ScriptId�� targetScriptId��� �����ϰ� ��ȭ�� �����ϵ��� ��.
            {
                StartDialog(scriptGroupId, targetScriptId, true);
                return true;
            }

            return false;
        });
    }
}
