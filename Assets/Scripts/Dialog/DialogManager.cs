using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance = null;

    private Sequence autoSkipSequence = null;
    private IDisposable skipStream = null;

    private TextManager textMgr;
    private EventManager eventMgr;

    public List<TweenObject> tweenList = new();

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

    private void Start()
    {
        ScriptManager.ReadScript();

        //DialogStart(10001);
    }

    public void DialogStart(int scriptID)
    {
        ScriptObject script = ScriptManager.GetScriptFromID(scriptID);

        ScriptManager.SetCurrentScript(script);

        ExecuteScript(script);
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
        //("ExecuteScript : " + script.scriptID).Log();

        if (script.isEvent == true)
        {
            if(script.eventData.eventType == EventType.Goto) //이 코드를 여기에 둬도 되나?? 230111
            {
                CreateEventSequence(script);
                return;
            }

            tweenList.Add(CreateEventSequence(script));
        }
        else
        {
            tweenList.Add(CreateTextSequence(script));

            //Audio 호출 관련 내용 추가할 것 230113
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

        ScriptObject nextScript = ScriptManager.GetNextScript();

        if(nextScript.isEvent == false)
        {
            return;
        }

        TweenObject nextEvent = CreateEventSequence(nextScript);

        tweenList.Add(nextEvent);

        ScriptManager.SetCurrentScript(nextScript);

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
                if (tweenObj.isInfinityLoop == true || tweenObj.isSkipped == true) //무한 루프 / 이미 스킵된 트윈은 플레이 중 여부를 고려하지 않음.
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

        skipStream.Dispose();

        ExecuteNextScript();
    }

    private void Skip(ScriptObject script)
    {
        //플레이 중인 트윈이 있는지 확인.
        bool isPlaying = ExistPlayingTween();

        isPlaying.Log();

        if (script.skipMethod == SkipMethod.Skipable && isPlaying == true)
        {
            DoAllTweens(tweenObj =>
            {
                tweenObj.Skip();
            });
        }
        else if (isPlaying == false)
        {
            Next();
        }
    }

    private void SetSkip(ScriptObject script)
    {
        if (script.skipMethod == SkipMethod.Auto)
        {
            Sequence skipSeq = DOTween.Sequence();
            autoSkipSequence = skipSeq;

            //가장 큰 duration 뽑기
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
        }
    }

    private void ExecuteNextScript()
    {
        ScriptManager.Next();

        if(skipStream != null)
        {
            skipStream.Dispose();
        }

        ExecuteScript(ScriptManager.currentScript);
    }
    #endregion

    #region Goto / MoveTo
    public void Goto(int scriptID)
    {
        //진행 중인 시퀀스, 스트림 모두 중지
        if(skipStream != null)
        {
            "Dispose".Log();
            skipStream.Dispose();
        }

        if(autoSkipSequence.IsActive() == true)
        {
            autoSkipSequence.Kill(false);
        }

        DoAllTweens(tween =>
        {
            tween.Skip(true);
            RemoveTween(tween);
        });

        DialogStart(scriptID);
    }

    public void ExecuteMoveTo(int targetID, Action<int> moveCB)
    {
        eventMgr.RemoveAllCharacter();

        int groupID = ScriptManager.GetGroupID(targetID);
        int startID = ScriptManager.GetFirstScriptIDFromGroupID(groupID);
        ScriptObject script = ScriptManager.GetScriptFromID(startID);
        ScriptManager.SetCurrentScript(script);

        MoveTo(script, targetID, moveCB);
    }

    private void MoveTo(ScriptObject script, int targetID, Action<int> moveCB)
    {
        if(script.scriptID > targetID)
        {
            moveCB(script.scriptID);
            return;
        }

        int scriptID = script.scriptID;

        float orgEventDuration = script.eventData.eventDuration;
        script.eventData.eventDuration = 0;

        float orgTextDuration = script.textDuration;
        script.textDuration = 0;

        if (script.isEvent == true)
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

        MoveTo(ScriptManager.Next(), targetID, moveCB);
    }
    #endregion
}