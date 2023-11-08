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
        foreach (TweenObject tweenObj in tweenList)
        {
            action(tweenObj);
        }
    }

    /// <summary>
    /// Tween을 삭제하더라도 문제가 생기지 않도록 역순회합니다.
    /// </summary>
    /// <param name="action"></param>
    public void DoAllTweensForModify(Action<TweenObject> action)
    {
        for (int i = tweenList.Count - 1; i >= 0; --i)
        {
            action(tweenList[i]);
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
    
    /// <summary>
    /// 모든 TweenObject의 Skip을 호출합니다.
    /// </summary>
    public void SkipAllTweens()
    {
        //DoAllTweens(tweenObj =>
        //{
        //    tweenObj.Skip(true);
        //});

        DoAllTweensForModify(tweenObj =>
        {
            tweenObj.Skip(true);
        });
    }

    public void StopAllTweens()
    {
        DoAllTweensForModify(tweenObj =>
        {
            tweenObj.Skip(true, true);
        });
    }

    public bool ExistPlayingTween()
    {
        bool isPlaying = false;

        DoAllTweens(tweenObj =>
        {
            if (isPlaying == true) return;

            if (tweenObj.tween.IsPlaying() == true)
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