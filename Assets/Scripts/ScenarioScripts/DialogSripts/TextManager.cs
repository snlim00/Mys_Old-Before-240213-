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

    public Text textForLength; //텍스트의 마지막 줄의 길이를 알기 위한 텍스트로, text와 완전히 동일한 설정의 텍스트 박스여야 함.

    /// <summary>
    /// 현재 입력된 텍스트의 마지막 줄이 적용된 텍스트 박스의 Width를 반환
    /// </summary>
    /// <returns></returns>
    public float GetLastLineTextLength()
    {
        return textForLength.rectTransform.sizeDelta.x;
    }

    /// <summary>
    /// 이벤트 스크립트로 시퀀스를 만들어 반환함.
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    public Sequence CreateTextSequence(ScriptObject script)
    {
        //캐릭터 이름에 따라 그림자의 색상 변경
        {
            if (script.characterName == "류지혜" || script.characterName == "지혜")
            {
                characterNameShadow.effectColor = new(255 / 255f, 240 / 255f, 197 / 255f, 0.5f);
                this.text.color = new(255 / 255f, 240 / 255f, 197 / 255f);
            }
            else if (script.characterName == "박윤하" || script.characterName == "윤하")
            {
                characterNameShadow.effectColor = new(241 / 255f, 205 / 255f, 255 / 255f, 0.5f);
                this.text.color = new(241 / 255f, 205 / 255f, 255 / 255f);
            }
            else if (script.characterName == "홍세은" || script.characterName == "세은")
            {
                characterNameShadow.effectColor = new(197 / 255f, 244 / 255f, 255 / 255f, 0.5f);
                this.text.color = new(197 / 255f, 244 / 255f, 255 / 255f);
            }
            else
            {
                characterNameShadow.effectColor = new(0, 0, 0, 0.5f);
                this.text.color = Color.white;
            }
        }

        Sequence seq = DOTween.Sequence();

        //치환해야할 텍스트 치환
        string msg = script.text.Replace("<br>", Environment.NewLine); //<br>을 \n으로 치환 
        msg = msg.Replace("<PlayerName>", GameData.saveFile.playerName);

        string name = script.characterName.Replace("<PlayerName>", GameData.saveFile.playerName);

        seq.AppendCallback(() => characterName.text = name);
        seq.AppendCallback(() => this.text.text = ""); //텍스트박스 비우기

        if (script.textDuration == 0) //textDuration이 0이라면 DOText를 실행하지 않고 그냥 텍스트를 설정하도록 함. (안 그러면 시퀀스가 좀 이상해지는 듯) 221219
        {
            seq.AppendCallback(() => this.text.text = msg);
        }
        else //textDuration은 한 글자가 생성되는 데 걸리는 시간이므로 text의 Length에 곱해서 사용.
        {
            seq.Append(this.text.DOText(msg, msg.Length * script.textDuration));
        }

        //텍스트의 마지막 줄만 가져와 textForLength의 텍스트에 적용
        string[] lines = msg.Split('\n');
        string lastLine = lines[lines.Length - 1];
        textForLength.text = lastLine;

        return seq;
    }

    /// <summary>
    /// 텍스트 관련 모든 변수 및 오브젝트 초기화
    /// </summary>
    /// 새로운 변수 및 오브젝트가 추가되면 해당 함수에서 초기화하도록 해줘야 함.
    public void ResetAll()
    {
        characterName.text = string.Empty;
        text.text = string.Empty;
        textTransform.anchoredPosition = Vector3.zero;

        text.alignment = TextAnchor.UpperLeft;
    }
}
