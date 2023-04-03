using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UniRx;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance { get; private set; } = null;

    private Sequence autoSkipSequence = null;
    private IDisposable skipStream = null;

    private TextManager textMgr;
    private EventManager eventMgr;

    public List<TweenObject> tweenList = new();

    public ScriptManager scriptMgr = new();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        textMgr = FindObjectOfType<TextManager>();
        eventMgr = FindObjectOfType<EventManager>();
    }

    #region Bounce Arrow
    [SerializeField] private Image bounceArrow;
    private Tween bounceArrowTween = null;
    private bool enableBounceArrow = false;

    private void SetBounceArrow(bool enable)
    {
        if(enable == true && enableBounceArrow == false)
        {
            bounceArrow.DOFade(1, 0.2f);

            float pos = bounceArrow.transform.localPosition.y - 7;
            bounceArrowTween = bounceArrow.transform.DOLocalMoveY(pos, 0.55f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            enableBounceArrow = true;
        }
        else if(enable == false && enableBounceArrow == true)
        {
            bounceArrowTween?.Kill();

            bounceArrow.DOFade(0, 0.2f);

            enableBounceArrow = false;
        }
    }
    #endregion

    public void ReadScript(int scriptGroupID)
    {
        scriptMgr = new();
        scriptMgr.ReadScript("ScriptTable" + scriptGroupID + ".CSV");
    }

    public void DialogStart(int scriptID)
    {
        ScriptObject script = scriptMgr.GetScriptFromID(scriptID);

        scriptMgr.SetCurrentScript(script);

        ExecuteScript(script);
        "DialogStart".Log();
    }

    public void RemoveTween(TweenObject tweenObj)
    {
        tweenList.Remove(tweenObj);
    }

    private void DoAllTweens(Action<TweenObject> action)
    {
        for(int i = tweenList.Count - 1; i >= 0; --i)
        {
            action(tweenList[i]);
        }
    }

    #region createSequence
    private TweenObject CreateTextSequence(ScriptObject script)
    {
        Tween tween = textMgr.CreateTextSequence(script);
        TweenObject tweenObj = new(tween, script);

        return tweenObj;
    }

    private TweenObject CreateEventSequence(ScriptObject script)
    {
        Tween tween = eventMgr.CreateEventSequence(script);
        TweenObject tweenObj = new(tween, script);

        if (script.eventData.durationTurn > 0)
        {
            tweenObj.durationTurn = script.eventData.durationTurn;
            tweenObj.remainingTurn = script.eventData.durationTurn;
        }

        return tweenObj;
    }
    #endregion

    #region ExecuteScript
    private void ExecuteScript(ScriptObject script)
    {
        script.scriptID.Log("ExecuteScript");
        //("ExecuteScript : " + script.scriptID).Log();

        if (script.scriptType == ScriptType.Event)
        {
            if(script.eventData.eventType == EventType.Goto) //�� �ڵ带 ���⿡ �ֵ� �ǳ�?? 230111
            {
                CreateEventSequence(script);
                return;
            }

            tweenList.Add(CreateEventSequence(script));
        }
        else if(script.scriptType == ScriptType.Text)
        {
            tweenList.Add(CreateTextSequence(script));

            //Audio ȣ�� ���� ���� �߰��� �� 230113
        }

        AppendLinkEvent(script);

        DoAllTweens(tweenObj =>
        {
            if (tweenObj.tween.IsPlaying() == false)
            {
                tweenObj.tween.Play();
            }

            tweenObj.isSkipped = false;
        });

        SetSkip(script);
    }

    private void AppendLinkEvent(ScriptObject script)
    {
        if (script.linkEvent == false)
        {
            return;
        }

        ScriptObject nextScript = scriptMgr.GetNextScript();

        if(nextScript.scriptType == ScriptType.Text)
        {
            return;
        }

        TweenObject nextEvent = CreateEventSequence(nextScript);

        tweenList.Add(nextEvent);

        scriptMgr.SetCurrentScript(nextScript);

        AppendLinkEvent(nextScript);
    }
    #endregion

    #region Skip
    private bool ExistPlayingTween()
    {
        bool isPlaying = false;

        foreach (var tweenObj in tweenList)
        {
            Tween tween = tweenObj.tween;

            if (tween.IsPlaying() == true)
            {
                if (tweenObj.isInfinityLoop == true || tweenObj.isSkipped == true) //���� ���� / �̹� ��ŵ�� Ʈ���� �÷��� �� ���θ� ������� ����.
                {
                    continue;
                }

                isPlaying = true;
                break;
            }
        }

        return isPlaying;
    }

    private void Next()
    {
        DoAllTweens(tweenObj =>
        {
            tweenObj.Skip(true);
        });

        skipStream?.Dispose();

        ExecuteNextScript();

        SetBounceArrow(false);
    }

    private void Skip(ScriptObject script)
    {
        //�÷��� ���� Ʈ���� �ִ��� Ȯ��.
        bool isPlaying = ExistPlayingTween();

        isPlaying.Log();

        if (script.skipMethod == SkipMethod.Skipable && isPlaying == true)
        {
            DoAllTweens(tweenObj =>
            {
                tweenObj.Skip();
            });

            SetBounceArrow(true);
        }
        else if (isPlaying == false)
        {
            Next();
        }
    }

    //���� ������� Duration�� �����Ͽ� ����ϵ��� �ؾ� �� 230403
    private float FindLongestDuration()
    {
        float duration = tweenList[0].tween.Duration() - tweenList[0].tween.position;

        for (int i = 1; i < tweenList.Count; ++i)
        {
            if (tweenList[i].tween.Duration() == Mathf.Infinity || tweenList[i].remainingTurn > 0)
            {
                continue;
            }

            if (duration < tweenList[i].tween.Duration() - tweenList[i].tween.position || duration == Mathf.Infinity)
            {
                duration = tweenList[i].tween.Duration() - tweenList[i].tween.position;
            }
            else if (tweenList[0].remainingTurn > 0)
            {
                duration = tweenList[i].tween.Duration() - tweenList[i].tween.position;
            }
        }

        return duration;
    }

    private void SetSkip(ScriptObject script)
    {
        if (script.skipMethod == SkipMethod.Auto)
        {
            Sequence skipSeq = DOTween.Sequence();
            autoSkipSequence = skipSeq;
            
            float duration = FindLongestDuration();
            
            float skipInterval = script.skipDelay;
            if (duration != Mathf.Infinity)
            {
                skipInterval += duration;
            }

            skipSeq.AppendInterval(skipInterval);
            ("Auto Skip in... " + skipInterval).Log();
            skipSeq.AppendCallback(Next);

            skipSeq.Play();
        }
        else
        {
            skipStream = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => Skip(script));

            float duration = FindLongestDuration();

            Sequence bounceArrowDelay = DOTween.Sequence();
            bounceArrowDelay.AppendInterval(duration);
            bounceArrowDelay.AppendCallback(() => SetBounceArrow(true));

            bounceArrowDelay.Play();
        }
    }

    private void ExecuteNextScript()
    {
        if(scriptMgr.GetNextScript() == null)
        {
            "��� ��ũ��Ʈ�� ����Ǿ����ϴ�.".LogError();
            return;
        }

        scriptMgr.Next();

        if(skipStream != null)
        {
            skipStream.Dispose();
        }

        ExecuteScript(scriptMgr.currentScript);
    }
    #endregion

    public void StopDialog()
    {
        StopAllSequence();
    }

    private void StopAllSequence()
    {
        //���� ���� ������, ��Ʈ�� ��� ����
        if (skipStream != null)
        {
            "Dispose".Log();
            skipStream.Dispose();
        }

        if (autoSkipSequence.IsActive() == true)
        {
            autoSkipSequence.Kill(false);
        }
    }

    public void ResetAll()
    {
        eventMgr.SetBackground(null);
        eventMgr.RemoveAllObject();

        textMgr.textBox.SetAlpha(1);
        textMgr.text.SetAlpha(1);
        textMgr.characterName.SetAlpha(1);
    }

    #region Goto / MoveTo
    public void Goto(int scriptID)
    {
        StopAllSequence();

        DoAllTweens(tween =>
        {
            tween.Skip(true);
            RemoveTween(tween);
        });

        DialogStart(scriptID);
    }

    public void ExecuteMoveTo(int targetID, Action<int> moveCB)
    {
        StopAllSequence();
        ResetAll();

        eventMgr.RemoveAllObject();

        int groupID = ScriptManager.GetGroupID(targetID);
        int startID = ScriptManager.GetFirstScriptIDFromGroupID(groupID);
        ScriptObject script = scriptMgr.GetScriptFromID(startID);
        scriptMgr.SetCurrentScript(script);

        MoveTo(script, targetID, moveCB);
    }

    //moveCB�� ���ڴ� scriptID
    private void MoveTo(ScriptObject script, int targetID, Action<int> moveAction)
    {
        //���� �ε�ȣ >= �Ǵ� >�� �ٲ㼭 �ش� ��ũ��Ʈ ������ �̵� or �ش� ��ũ��Ʈ�� �Ϸ�� ���·� �̵� ���� ����
        if(script.scriptID >= targetID)
        {
            moveAction(script.scriptID);
            return;
        }

        int scriptID = script.scriptID;

        float orgEventDuration = script.eventData.eventDuration;
        script.eventData.eventDuration = 0;

        float orgTextDuration = script.textDuration;
        script.textDuration = 0;

        if (script.scriptType == ScriptType.Event)
        {
            if (script.eventData.eventType == EventType.Goto)
            {
                CreateEventSequence(script);
                return;
            }

            tweenList.Add(CreateEventSequence(script));
        }
        else
        {
            tweenList.Add(CreateTextSequence(script));
        }

        script.eventData.eventDuration = orgEventDuration;
        script.textDuration = orgTextDuration;

        DoAllTweens(tweenObj =>
        {
            if (tweenObj.tween.IsPlaying() == false)
            {
                tweenObj.tween.Play();
            }

            tweenObj.isSkipped = false;
        });

        MoveTo(scriptMgr.Next(), targetID, moveAction);
    }
    #endregion
}