using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private NicknameScene nicknameScene;

    [SerializeField] private List<Sprite> tutorialSprites;
    [SerializeField] private Image tutorialImg;

    Sequence tutorialSeq;
    public bool doPhoneTutorial = false;

    // Start is called before the first frame update
    void Awake()
    {
        var chapterData = GameData.saveFile.chapterData;

        if (chapterData[CharacterInfo.Public] == 0)
        {
            nicknameScene.gameObject.SetActive(true);
            nicknameScene.Init();
        }
        else if (chapterData[CharacterInfo.Jihyae] <= 0 && chapterData[CharacterInfo.Yunha] <= 0 && chapterData[CharacterInfo.Seeun] <= 0)
        {
            doPhoneTutorial = true;
            tutorialImg.gameObject.SetActive(true);
        }
    }

    public void ActiveTutorialImage(int idx, bool active)
    {
        if(tutorialSeq?.IsPlaying() == true)
        {
            tutorialSeq.Kill();
            tutorialSeq = null;
        }

        tutorialSeq = DOTween.Sequence();

        tutorialSeq.Append(tutorialImg.DOFade(0, 0.5f).SetEase(Ease.OutCubic));
        tutorialSeq.AppendCallback(() => tutorialImg.sprite = tutorialSprites[idx]);
        tutorialSeq.Append(tutorialImg.DOFade(1, 0.5f).SetEase(Ease.OutCubic));

        tutorialSeq.Play();
    }
}
