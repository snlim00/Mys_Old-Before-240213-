using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private AudioClip bgm;

    [SerializeField] private Image creditScene;
    [SerializeField] private Text creditTxt;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.PlayBGM(bgm);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShowCredit();
        }
    }

    public void OnCloseScenario()
    {
        var chapterData = GameData.saveFile.chapterData;

        if (chapterData[CharacterInfo.Jihyae] >= 3 && chapterData[CharacterInfo.Yunha] >= 3 && chapterData[CharacterInfo.Seeun] >= 3)
        {
            ShowCredit();
        }
    }

    private void ShowCredit()
    {
        creditScene.gameObject.SetActive(true);

        creditScene.SetAlpha(0);

        Sequence seq = DOTween.Sequence();

        seq.Append(creditScene.DOFade(0.9f, 2f).SetEase(Ease.OutCubic));
        seq.Append(creditTxt.rectTransform.DOAnchorPosY(2150, 20f));
        seq.Append(creditScene.DOFade(0, 4f).SetEase(Ease.OutCubic));
        seq.AppendCallback(() => creditScene.gameObject.SetActive(false));

        seq.Play();
    }
}
