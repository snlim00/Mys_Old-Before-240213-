using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;

public class TweenManager
{
    private List<TweenObject> tweenList = new();

    public void AddRange(List<TweenObject> tweenObjs)
    {
        tweenList.AddRange(tweenObjs);
    }

    public void RemoveTween(TweenObject tweenObj)
    {
        tweenList.Remove(tweenObj);
    }

    public void DoAllTweens(Action<TweenObject> action)
    {
        foreach(TweenObject tweenObj in tweenList)
        {
            action(tweenObj);
        }
    }

    /// <summary>
    /// Play가 호출되지 않은 모든 트윈의 Play를 호출합니다.
    /// </summary>
    public void PlayAllTweens()
    {
        DoAllTweens(tweenObj =>
        {
            if (tweenObj.tween.IsPlaying() == false)
            {
                tweenObj.tween.Play();
            }

            tweenObj.isSkipped = false;
        });
    }

    public void SkipAllTweens()
    {
        //DoAllTweens(tweenObj =>
        //{
        //    tweenObj.Skip(true);
        //});

        for(int i = tweenList.Count - 1; i >= 0; --i)
        {
            tweenList[i].Skip(true);
        }
    }

    public bool ExistPlayingTween()
    {
        bool isPlaying = false;

        DoAllTweens(tweenObj =>
        {
            if (isPlaying == true) return;

            if(tweenObj.tween.IsPlaying() == true)
            {
                if (tweenObj.isInfinityLoop == true || tweenObj.isSkipped == true) //무한 루프, 이미 스킵된 트윈은 플레이 중 여부를 고려하지 않음.
                {
                    return;
                }

                isPlaying = true;
            }
        });

        return isPlaying;
    }

    //추후 오디오의 Duration을 포함하여 계산하도록 할 것. 230425
    /// <summary>
    /// 가장 긴 남은 지속 시간을 반환합니다.<br></br>
    /// 무한 반복 트윈과 현재 턴에서 종료되지 않는 트윈은 제외됩니다.
    /// </summary>
    public float FindLongestDuration()
    {
        float duration = -1;

        DoAllTweens(tweenObj =>
        {
            Tween tween = tweenObj.tween;

            if (tween.Duration() == Mathf.Infinity || tweenObj.remainingTurn > 0) //무한 반복 트윈, 현재 턴에서 종료되지 않는 트윈은 제외함.
            {
                return;
            }

            if (duration < tweenObj.GetRemainingDuration() || duration == -1) //가장 큰 남은 시간을 duration에 할당.
            {
                duration = tweenObj.GetRemainingDuration();
            }
        });

        return duration;
    }
}

public class TweenObject
{
    private TweenManager tweenMgr;

    public Tween tween;

    public ScriptObject script;

    public int durationTurn = 0; //해당 트윈이 사리지기까지 남은 총 턴. (최초 한 번 초기화 이후 건드리지 않음)
    public int remainingTurn = 0; //해당 트윈이 사라지기까지 남은 턴.
    public bool isSkipped = false;

    public bool isInfinityLoop
    {
        get { return tween.Loops() == -1; }
    }


    public TweenObject(Tween tween, ScriptObject script)
    {
        this.tween = tween;
        this.script = script;
    }

    public TweenObject(Tween tween, ScriptObject script, TweenManager tweenMgr)
    {
        this.tween = tween;
        this.script = script;
        this.tweenMgr = tweenMgr;
    }

    /// <summary>
    /// 스킵을 진행합니다. 남은 턴을 감소시키거나 시퀀스를 중단합니다.
    /// </summary>
    /// <param name="completeInfinityLoop"></param>
    /// <param name="ignoreRemainingTurn"></param>
    public void Skip(bool completeInfinityLoop = false, bool ignoreRemainingTurn = false)
    {
        if (script.scriptType == ScriptType.Text)
        {
            Complete(completeInfinityLoop);
            return;
        }

        if (remainingTurn > 0 && isSkipped == false) //남은 턴이 존재하며, 아직 스킵되지 않은 이벤트라면 턴만 감소시키고 스킵하지 않음.
        {
            if(ignoreRemainingTurn == true)
            {
                Complete(completeInfinityLoop);
            }
            else
            {
                remainingTurn -= 1;
                isSkipped = true;
            }
        }
        else if (remainingTurn <= 0)
        {
            Complete(completeInfinityLoop);
        }
    }

    private void Complete(bool completeInfinityLoop = false)
    {
        if(isInfinityLoop == true && completeInfinityLoop == true)
        {
            tween.Goto(tween.Duration(false));
            tween.Pause();
            tweenMgr.RemoveTween(this);
        }
        else
        {
            tween.Complete(true);
            tweenMgr.RemoveTween(this);
        }
    }

    public float GetRemainingDuration()
    {
        return tween.Duration() - tween.position;
    }
}