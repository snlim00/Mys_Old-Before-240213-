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
        if(script.characterName == "������" || script.characterName == "����")
        {
            characterNameShadow.effectColor = new(255 / 255f, 240 / 255f, 197 / 255f, 0.5f);
            this.text.color = new(255 / 255f, 240 / 255f, 197 / 255f);
        }
        else if(script.characterName == "������" || script.characterName == "����")
        {
            characterNameShadow.effectColor = new(241 / 255f, 205 / 255f, 255 / 255f, 0.5f);
            this.text.color = new(241 / 255f, 205 / 255f, 255 / 255f);
        }
        else if(script.characterName == "ȫ����" || script.characterName == "����")
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
        seq.AppendCallback(() => this.text.text = ""); //�ؽ�Ʈ�ڽ� ����

        if (script.textDuration == 0) //textDuration�� 0�̶�� DOText�� �������� �ʰ� �׳� �ؽ�Ʈ�� �����ϵ��� ��. (�� �׷��� �������� �� �̻������� ��) 221219
        {
            seq.AppendCallback(() => this.text.text = text);
        }
        else //textDuration�� �� ���ڰ� �����Ǵ� �� �ɸ��� �ð��̹Ƿ� text�� Length�� ���ؼ� ���.
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
