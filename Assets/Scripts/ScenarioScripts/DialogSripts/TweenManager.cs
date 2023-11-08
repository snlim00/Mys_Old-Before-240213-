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
    /// Tween�� �����ϴ��� ������ ������ �ʵ��� ����ȸ�մϴ�.
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
    /// Play�� ȣ����� ���� ��� Ʈ���� Play�� ȣ���մϴ�.
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
    /// ��� TweenObject�� Skip�� ȣ���մϴ�.
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
                if (tweenObj.isInfinityLoop == true || tweenObj.isSkipped == true) //���� ����, �̹� ��ŵ�� Ʈ���� �÷��� �� ���θ� ������� ����.
                {
                    return;
                }

                isPlaying = true;
            }
        });

        return isPlaying;
    }

    //���� ������� Duration�� �����Ͽ� ����ϵ��� �� ��. 230425
    /// <summary>
    /// ���� �� ���� ���� �ð��� ��ȯ�մϴ�.<br></br>
    /// ���� �ݺ� Ʈ���� ���� �Ͽ��� ������� �ʴ� Ʈ���� ���ܵ˴ϴ�.
    /// </summary>
    public float FindLongestDuration()
    {
        float duration = -1;

        DoAllTweens(tweenObj =>
        {
            Tween tween = tweenObj.tween;

            if (tween.Duration() == Mathf.Infinity || tweenObj.remainingTurn > 0) //���� �ݺ� Ʈ��, ���� �Ͽ��� ������� �ʴ� Ʈ���� ������.
            {
                return;
            }

            if (duration < tweenObj.GetRemainingDuration() || duration == -1) //���� ū ���� �ð��� duration�� �Ҵ�.
            {
                duration = tweenObj.GetRemainingDuration();
            }
        });

        return duration;
    }
}