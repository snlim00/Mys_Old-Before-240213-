using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;

public class TweenObject
{
    private TweenManager tweenMgr;

    public Tween tween;

    public ScriptObject script;

    public int durationTurn = 0; //해당 트윈이 사리지기까지 남은 총 턴. (최초 한 번 초기화 이후 건드리지 않음)
    public int remainingTurn = 0; //해당 트윈이 사라지기까지 현재 남은 턴.
    public bool isSkipped = false;

    public bool isInfinityLoop => tween.Loops() == -1;


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