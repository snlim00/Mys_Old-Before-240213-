using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System;
using System.Reflection;
using System.Linq;

public class TextManager : MonoBehaviour
{
    [SerializeField] private Text textBox;

    [SerializeField] private Text characterName;

    public Sequence CreateTextSequence(ScriptObject script)
    {
        string characterName = script.characterName;
        this.characterName.text = characterName;

        string text = script.text;
        float textDuration = script.textDuration;

        float skipDuration = script.skipDelay;

        Sequence textSeq = DOTween.Sequence();
        textSeq.AppendCallback(() => textBox.text = "");

        if (textDuration == 0) //textDuration이 0이라면 DOText를 실행하지 않고 그냥 텍스트를 설정하도록 함. (안 그러면 시퀀스가 좀 이상해지는 듯) 221219
        {
            textSeq.AppendCallback(() => textBox.text = text);
        }
        else //textDuration은 한 글자가 생성되는 데 걸리는 시간이므로 text의 Length에 곱해서 사용.
        {
            textSeq.Append(textBox.DOText(text, text.Length * textDuration));
        }

        return textSeq;
    }
}
