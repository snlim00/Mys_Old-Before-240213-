using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LetterBoxUI : MonoBehaviour
{
    [SerializeField] private Image selectEffect;

    //[SerializeField] private Button mapButton;
    //[SerializeField] private Button optionButton;

    [SerializeField] private Button mainButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button planningButton;
    [SerializeField] private Button settingButton;

    private Sequence selecteSequence;

    private void Start()
    {
        mainButton.onClick.AddListener(() => OnMenuButtonClick(mainButton, OnMainButtonClick));

        saveButton.onClick.AddListener(() => OnMenuButtonClick(saveButton, OnSaveButtonClick));

        loadButton.onClick.AddListener(() => OnMenuButtonClick(loadButton, OnLoadButtonClick));

        planningButton.onClick.AddListener(() => OnMenuButtonClick(planningButton, OnPlanningButtonClick));

        settingButton.onClick.AddListener(() => OnMenuButtonClick(settingButton, OnSettingButtonClick));
    }

    private void OnMenuButtonClick(Button btn, Action action)
    {
        if(selecteSequence?.IsPlaying() == true) { return; }

        var pos = btn.transform.localPosition;

        pos.y = -4.5f;

        selecteSequence = DOTween.Sequence();

        selecteSequence.Append(selectEffect.transform.DOScaleX(0, 0.1f).SetEase(Ease.OutCubic));
        selecteSequence.AppendInterval(0.1f);
        selecteSequence.AppendCallback(() => {
            selectEffect.transform.localPosition = pos;
            action?.Invoke();
            });
        selecteSequence.Append(selectEffect.transform.DOScaleX(1, 0.15f).SetEase(Ease.OutCubic));

        selecteSequence.Play();
    }

    private void OnMainButtonClick()
    {

    }

    private void OnSaveButtonClick()
    {

    }

    private void OnLoadButtonClick()
    {

    }

    private void OnPlanningButtonClick()
    {

    }

    private void OnSettingButtonClick()
    {

    }
}
