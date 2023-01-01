using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UniRx;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEditor.PackageManager;

public class DialogManager : MonoBehaviour
{
    private IDisposable skipStream = null;

    private TextManager textMgr;
    private EventManager eventMgr;

    private void Awake()
    {
        textMgr = FindObjectOfType<TextManager>();
        eventMgr = FindObjectOfType<EventManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ScriptManager.ReadScript();

        ScriptManager.SetScriptFromID(10001);


        ScriptObject currentScript = ScriptManager.GetCurrentScript();

        ExecuteScript(currentScript);
    }

    public void ExecuteScript(ScriptObject script)
    {
        ("ExecuteScript: " + script.scriptID).Log();

        bool isEvent = script.isEvent;

        Sequence sequence = null;

        if (isEvent == true)
        {
            sequence = CreateEventSequence(script); //스크립트 종료도 이벤트에서 처리할 것임. 221223
        }
        else
        {
            sequence = CreateTextSequence(script);
        }

        //스킵 처리
        if (script.skipMethod == SkipMethod.Auto)
        {
            sequence.AppendInterval(script.skipDelay);
            //스킵 딜레이로 하는 것도 좋은데 텍스트가 이벤트에 비해 너무 짧은 경우에 대한 처리 필요함!! 221223
            //(AppendINterval이 아닌 Insert로 한 후 가장 긴 시간을 넣는 것도 방법일 듯)
            //스킵할 때는 이게 문제되지 않음 221224 (이벤트, 텍스트 관계 없이 하나의 시퀀스를 사용하도록 한 이후 알아서 가장 느린 연출 뒤로 가게 됨.)
            
            sequence.AppendCallback(() => NextScript());
        }
        else
        {
            CreateSkipStream(script, sequence);
        }

        //연결 이벤트 시퀀스에 추가
        AppendNextEvent(script, sequence);

        //시퀀스 실행
        sequence.Play();
    }

    private Sequence CreateTextSequence(ScriptObject script)
    {
        Sequence textSeq = textMgr.CreateTextSequence(script);

        return textSeq;
    }

    private Sequence CreateEventSequence(ScriptObject script)
    {
        Sequence eventSeq = eventMgr.CreateEventSequence(script);

        return eventSeq;
    }

    private void AppendNextEvent(ScriptObject script, Sequence sequence)
    {
        if (script.linkEvent == false) return;

        ScriptManager.Next();
        ScriptObject nextScript = ScriptManager.GetCurrentScript();

        if (nextScript.isEvent == false) return;

        Sequence nextEvent = eventMgr.CreateEventSequence(nextScript);

        sequence.Insert(0, nextEvent);

        "AppendNextEvent".Log();

        AppendNextEvent(nextScript, sequence);
    }

    private void CreateSkipStream(ScriptObject script, Sequence sequence)
    {
        skipStream = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => Skip(script, sequence));
    }

    private void Skip(ScriptObject script, Sequence sequence)
    {
        if (sequence.IsPlaying() && script.skipMethod == SkipMethod.Skipable)
        {
            //sequence.Kill(true);
            sequence.Complete();
            //sequence.Pause();
        }
        else if (sequence.IsPlaying() == false)
        {
            NextScript();
        }
    }

    private void NextScript()
    {
        ScriptManager.Next();

        if(skipStream != null)
        {
            skipStream.Dispose();
        }


        ExecuteScript(ScriptManager.GetCurrentScript());


        "다음 스크립트".로그();
    }
}
