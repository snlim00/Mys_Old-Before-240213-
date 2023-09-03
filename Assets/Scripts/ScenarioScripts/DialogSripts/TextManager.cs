using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public RectTransform textTransform;

    public Image textBox;
    public Text text;
    public Text characterName;
    public Shadow characterNameShadow;

    public Sequence CreateTextSequence(ScriptObject script)
    {
        if(script.characterName == "류지혜" || script.characterName == "지혜")
        {
            characterNameShadow.effectColor = new(255 / 255f, 240 / 255f, 197 / 255f, 0.5f);
            this.text.color = new(255 / 255f, 240 / 255f, 197 / 255f);
        }
        else if(script.characterName == "박윤하" || script.characterName == "윤하")
        {
            characterNameShadow.effectColor = new(241 / 255f, 205 / 255f, 255 / 255f, 0.5f);
            this.text.color = new(241 / 255f, 205 / 255f, 255 / 255f);
        }
        else if(script.characterName == "홍세은" || script.characterName == "세은")
        {
            characterNameShadow.effectColor = new(197 / 255f, 244 / 255f, 255 / 255f, 0.5f);
            this.text.color = new(197 / 255f, 244 / 255f, 255 / 255f);
        }
        else
        {
            characterNameShadow.effectColor = new(0, 0, 0, 0.5f);
            this.text.color = Color.white;
        }

        Sequence seq = DOTween.Sequence();

        string text = script.text.Replace("<br>", Environment.NewLine);
        text = text.Replace("<PlayerName>", GameData.saveFile.playerName);

        seq.AppendCallback(() => characterName.text = script.characterName);
        seq.AppendCallback(() => this.text.text = ""); //텍스트박스 비우기

        if (script.textDuration == 0) //textDuration이 0이라면 DOText를 실행하지 않고 그냥 텍스트를 설정하도록 함. (안 그러면 시퀀스가 좀 이상해지는 듯) 221219
        {
            seq.AppendCallback(() => this.text.text = text);
        }
        else //textDuration은 한 글자가 생성되는 데 걸리는 시간이므로 text의 Length에 곱해서 사용.
        {
            seq.Append(this.text.DOText(text, text.Length * script.textDuration));
        }

        return seq;
    }

    public void ResetAll()
    {
        characterName.text = string.Empty;
        text.text = string.Empty;
        textTransform.anchoredPosition = Vector3.zero;

        text.alignment = TextAnchor.UpperLeft;
    }
}
