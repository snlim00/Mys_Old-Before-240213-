using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChoiceOptionButton : MonoBehaviour
{
    private Button btn;
    private Text text;

    private ChoiceInfo choiceInfo;

    private void Awake()
    {
        btn = this.GetComponent<Button>();
        text = this.GetComponentInChildren<Text>();
    }

    public void Init(ChoiceInfo choiceInfo, Action btnCb)
    {
        this.choiceInfo = choiceInfo;

        text.text = choiceInfo.text;

        if(GameData.saveFile.GetConditionData(choiceInfo) < choiceInfo.conditionAmount)
        {
            btn.interactable = false;
        }

        btn.onClick.AddListener(() =>
        {
            DialogManager.Instance.Goto(choiceInfo.scriptId);
            btnCb();
        });

        btn.image.SetAlpha(0);
        text.SetAlpha(0);
    }

    public void FadeIn(EventData eventData, float delay)
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(delay);
        seq.Append(btn.image.DOFade(1, eventData.eventDuration));
        seq.Join(text.DOFade(1, eventData.eventDuration));

        seq.Play();
    }

    public void FadeOut()
    {
        btn.interactable = false;

        Sequence seq = DOTween.Sequence();

        seq.Append(btn.image.DOFade(0, 0.4f));
        seq.Join(text.DOFade(0, 0.4f));
        seq.AppendCallback(() => Destroy(this.gameObject));

        seq.Play();
    }
}
