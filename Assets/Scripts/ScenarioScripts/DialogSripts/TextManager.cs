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

    public Sequence CreateTextSequence(ScriptObject script)
    {
        Sequence seq = DOTween.Sequence();

        string text = script.text.Replace("<br>", Environment.NewLine);

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
    }
}
