﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEditor;
using UniRx;

public class TweenObject
{
    public Tween tween;

    public ScriptObject script;

    public int durationTurn = 0; //해당 트윈이 사리지기까지 남은 총 턴. (최초 한 번 초기화 이후 건드리지 않음)
    public int remainingTurn = 0; //해당 트윈이 사라지기까지 남은 턴.
    public bool isSkipped = false;

    public bool isInfinityLoop
    {
        get { return tween.Loops() == -1; }
    }

    public void Skip(bool completeInfinityLoop = false)
    {
        if (script.isEvent == false)
        {
            tween.Complete(true);
            DialogManager.instance.RemoveTween(this);
            return;
        }

        if (remainingTurn > 0 && isSkipped == false) //남은 턴이 존재하며, 아직 스킵되지 않은 이벤트라면 턴만 감소시키고 스킵하지 않음.
        {
            //"턴 감소".Log();
            remainingTurn -= 1;
            isSkipped = true;
        }
        else if (remainingTurn <= 0)
        {
            if (isInfinityLoop == false)
            {
                tween.Complete(true);
                //"Complete 1".Log();
                DialogManager.instance.RemoveTween(this);
            }
            else
            {
                if (completeInfinityLoop == true)
                {
                    tween.Goto(tween.Duration(false));
                    tween.Pause();
                    //"Complete 2".Log();
                    DialogManager.instance.RemoveTween(this);
                }
            }
        }
    }

    public TweenObject(Tween tween, ScriptObject script)
    {
        this.tween = tween;
        this.script = script;
    }
}