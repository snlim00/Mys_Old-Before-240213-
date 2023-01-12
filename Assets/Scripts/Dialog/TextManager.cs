using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [SerializeField] private Text textBox;
    [SerializeField] private Text characterName;

    public Sequence CreateTextSequence(ScriptObject script)
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => textBox.text = ""); //텍스트박스 비우기

        if (script.textDuration == 0) //textDuration이 0이라면 DOText를 실행하지 않고 그냥 텍스트를 설정하도록 함. (안 그러면 시퀀스가 좀 이상해지는 듯) 221219
        {
            seq.AppendCallback(() => textBox.text = script.text);
        }
        else //textDuration은 한 글자가 생성되는 데 걸리는 시간이므로 text의 Length에 곱해서 사용.
        {
            seq.Append(textBox.DOText(script.text, script.text.Length * script.textDuration));
        }

        return seq;
    }
}
