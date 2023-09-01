using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum PhoneSceneList
{
    Home,
    TalkMain,
    TalkProfile,
}

public class PhoneManager : Singleton<PhoneManager>
{
    private RectTransform rectTransform;

    [SerializeField] private GameObject talkNotice;
    [SerializeField] private Button takeOutBtn;

    private bool isEnable = false;

    private Vector2 defaultPosition;

    public HomeScene homeScene;
    public TalkMainScene talkMainScene;
    public TalkProfileScene talkProfileScene;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultPosition = rectTransform.anchoredPosition;

        takeOutBtn.onClick.AddListener(OnTakeOutPhoneButtonClick);
    }

    public void OpenScene(PhoneSceneList scene)
    {
        CloseAllScene();

        switch(scene)
        {
            case PhoneSceneList.Home:
                homeScene.gameObject.SetActive(true);
                homeScene.OnOpenScene();
                break;

            case PhoneSceneList.TalkMain:
                talkMainScene.gameObject.SetActive(true);
                talkMainScene.OnOpenScene();
                break;

            case PhoneSceneList.TalkProfile:
                talkProfileScene.gameObject.SetActive(true);
                talkProfileScene.OnOpenScene();
                break;
        }
    }

    private void CloseAllScene()
    {
        foreach(var scene in Enum.GetValues(typeof(PhoneSceneList)))
        {
            CloseScene((PhoneSceneList)scene);
        }
    }

    private void CloseScene(PhoneSceneList scene)
    {
        switch (scene)
        {
            case PhoneSceneList.Home:
                if (homeScene.gameObject.activeSelf == true)
                {
                    homeScene.OnCloseScene();
                    homeScene.gameObject.SetActive(false);
                }
                break;

            case PhoneSceneList.TalkMain:
                if (talkMainScene.gameObject.activeSelf == true)
                {
                    talkMainScene.OnCloseScene();
                    talkMainScene.gameObject.SetActive(false);
                }
                break;

            case PhoneSceneList.TalkProfile:
                if (talkProfileScene.gameObject.activeSelf == true)
                {
                    talkProfileScene.OnCloseScene();
                    talkProfileScene.gameObject.SetActive(false);
                }
                break;
        }
    }

    private void OnTakeOutPhoneButtonClick()
    {
        if(isEnable == true) { return; }

        OpenScene(PhoneSceneList.Home);

        isEnable = true;

        Sequence seq = DOTween.Sequence();

        seq.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - 250, 0.2f).SetEase(Ease.OutCubic));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => {
            rectTransform.anchoredPosition = new(0, 800);
            transform.localScale = Vector2.one * 1.15f;
        });
        seq.Append(rectTransform.DOAnchorPosY(15, 0.3f).SetEase(Ease.OutQuart));

        seq.Play();
    }

    public void OnPutInPhoneButtonClick()
    {
        if (isEnable == false) { return; }

        OpenScene(PhoneSceneList.Home);

        isEnable = false;

        Sequence seq = DOTween.Sequence();

        seq.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 800, 0.2f).SetEase(Ease.OutCubic));
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => {
            var pos = defaultPosition;
            pos.y -= 250;
            rectTransform.anchoredPosition = pos;
            transform.localScale = Vector2.one;
        });
        seq.Append(rectTransform.DOAnchorPosY(defaultPosition.y, 0.3f).SetEase(Ease.OutQuart));

        seq.Play();
    }
}
