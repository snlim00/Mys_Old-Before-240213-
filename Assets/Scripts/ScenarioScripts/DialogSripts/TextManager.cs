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

    public Text textForLength; //�ؽ�Ʈ�� ������ ���� ���̸� �˱� ���� �ؽ�Ʈ��, text�� ������ ������ ������ �ؽ�Ʈ �ڽ����� ��.

    /// <summary>
    /// ���� �Էµ� �ؽ�Ʈ�� ������ ���� ����� �ؽ�Ʈ �ڽ��� Width�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public float GetLastLineTextLength()
    {
        return textForLength.rectTransform.sizeDelta.x;
    }

    /// <summary>
    /// �̺�Ʈ ��ũ��Ʈ�� �������� ����� ��ȯ��.
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    public Sequence CreateTextSequence(ScriptObject script)
    {
        //ĳ���� �̸��� ���� �׸����� ���� ����
        {
            if (script.characterName == "������" || script.characterName == "����")
            {
                characterNameShadow.effectColor = new(255 / 255f, 240 / 255f, 197 / 255f, 0.5f);
                this.text.color = new(255 / 255f, 240 / 255f, 197 / 255f);
            }
            else if (script.characterName == "������" || script.characterName == "����")
            {
                characterNameShadow.effectColor = new(241 / 255f, 205 / 255f, 255 / 255f, 0.5f);
                this.text.color = new(241 / 255f, 205 / 255f, 255 / 255f);
            }
            else if (script.characterName == "ȫ����" || script.characterName == "����")
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

        //ġȯ�ؾ��� �ؽ�Ʈ ġȯ
        string msg = script.text.Replace("<br>", Environment.NewLine); //<br>�� \n���� ġȯ 
        msg = msg.Replace("<PlayerName>", GameData.saveFile.playerName);

        string name = script.characterName.Replace("<PlayerName>", GameData.saveFile.playerName);

        seq.AppendCallback(() => characterName.text = name);
        seq.AppendCallback(() => this.text.text = ""); //�ؽ�Ʈ�ڽ� ����

        if (script.textDuration == 0) //textDuration�� 0�̶�� DOText�� �������� �ʰ� �׳� �ؽ�Ʈ�� �����ϵ��� ��. (�� �׷��� �������� �� �̻������� ��) 221219
        {
            seq.AppendCallback(() => this.text.text = msg);
        }
        else //textDuration�� �� ���ڰ� �����Ǵ� �� �ɸ��� �ð��̹Ƿ� text�� Length�� ���ؼ� ���.
        {
            seq.Append(this.text.DOText(msg, msg.Length * script.textDuration));
        }

        //�ؽ�Ʈ�� ������ �ٸ� ������ textForLength�� �ؽ�Ʈ�� ����
        string[] lines = msg.Split('\n');
        string lastLine = lines[lines.Length - 1];
        textForLength.text = lastLine;

        return seq;
    }

    /// <summary>
    /// �ؽ�Ʈ ���� ��� ���� �� ������Ʈ �ʱ�ȭ
    /// </summary>
    /// ���ο� ���� �� ������Ʈ�� �߰��Ǹ� �ش� �Լ����� �ʱ�ȭ�ϵ��� ����� ��.
    public void ResetAll()
    {
        characterName.text = string.Empty;
        text.text = string.Empty;
        textTransform.anchoredPosition = Vector3.zero;

        text.alignment = TextAnchor.UpperLeft;
    }
}
