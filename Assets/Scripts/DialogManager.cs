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

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    private Text textBox;

    [SerializeField]
    private Text characterName;

    private IDisposable skipStream;

    // Start is called before the first frame update
    void Start()
    {
        ScriptManager.scripts = CSVReader.ReadScript("Data/ScriptTable.CSV");

        ScriptManager.GetCurrentScript();


        ExecuteScript();
    }

    public void ExecuteScript()
    {
        ScriptObject currentScript = ScriptManager.GetCurrentScript();

        float skipDuration = currentScript.skipDuration;
        string eventType = currentScript.eventType;

        bool isEvent = false;

        if (eventType != null && eventType.Length != 0)
        {
            isEvent = true;
        }

        if (isEvent == true)
        {
            //var eventParam = currentScript.eventParam;

            //MethodInfo eventMethod = this.GetType().GetMethod(eventType);
            //eventMethod.Invoke(this, eventParam);
        }
        else
        {
            ExecuteDialog(currentScript);
        }
    }

    private void ExecuteDialog(ScriptObject script)
    {
        string characterName = script.characterName;
        this.characterName.text = characterName;

        string text = script.text;
        float textDuration = script.textDuration;

        float skipDuration = script.skipDuration;

        var textSequence = DOTween.Sequence().Pause();
        textSequence.AppendCallback(() => textBox.text = "");

        if (textDuration == 0) //textDuration이 0이라면 DOText를 실행하지 않고 그냥 텍스트를 설정하도록 함. (안 그러면 시퀀스가 좀 이상해지는 듯) 221219
        {
            textSequence.AppendCallback(() => textBox.text = text);
        }
        else //textDuration은 한 글자가 생성되는 데 걸리는 시간이므로 text의 Length에 곱해서 사용.
        {
            textSequence.Append(textBox.DOText(text, text.Length * textDuration).SetEase(Ease.Linear)); //SetEase를 해주지 않으면 기본적으로 다른 방식의 Ease가 적용됨. 221219
        }

        textSequence.AppendCallback(() => "텍스트 시퀀스 종료".로그());
        var skipSequence = DOTween.Sequence().Pause();
        skipSequence.AppendInterval(skipDuration);
        skipSequence.AppendCallback(() => NextScript()); //skipDuration 뒤에 스킵 가능한 상태로 만들 것. (현재는 skipDuration 뒤에 자동으로 스킵됨.)


        if (script.skipMethod == SkipMethod.Auto)
        {
            //textSequence.AppendCallback(() => NextScript());
        }
        else
        {
            textSequence.Append(skipSequence);
        }

        textSequence.Play();


        //스킵 옵저버 등록
        skipStream = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => Skip(script, textSequence));
    }

    private void Skip(ScriptObject script, Sequence textSequence)
    {
        if (textSequence.IsActive() && script.skipMethod == SkipMethod.Skipable)
        {
            "텍스트 연출 스킵".Log();
            textSequence.Kill(true);
        }
        else if (textSequence.IsActive() == false)
        {
            "다음 대사로 이동".Log();
            NextScript();
        }
    }

    private void NextScript()
    {
        ScriptManager.Next();
        skipStream.Dispose();

        ExecuteScript();

        "다음 스크립트".로그();
    }

    public void CloseScript(Action callback)
    {
        //scriptSequence.Kill(true);
        //skipStream.Dispose();

        //callback();
    }
}
