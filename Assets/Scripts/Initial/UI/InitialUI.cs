using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InitialUI : MonoBehaviour
{
    private Sequence sideBarSeq;

    private bool activeSideBar = false;
    [SerializeField] private Button sideBarBtn;
    [SerializeField] private Image sideBar;
    [SerializeField] private Button startBtn;

    [SerializeField] private Image background;
    [SerializeField] private Image logo;

    [SerializeField] private Image arrowEffect;

    public const float aniDuration = 0.4f;
    private Ease aniEase = Ease.OutQuad;

    private void Start()
    {
        startBtn.onClick.AddListener(() => MysSceneManager.LoadLobbyScene(null));
        sideBarBtn.onClick.AddListener(OnSideBarButtonClick);

        arrowEffect.rectTransform.DOAnchorPosX(-260, 1).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Play();
    }

    private void OnSideBarButtonClick()
    {
        if(sideBarSeq?.IsPlaying() == true)
        {
            return;
        }

        if (activeSideBar == false)
        {
            ActiveSideBar();
        }
        else
        {
            DisableSideBar();
        }
    }

    private void ActiveSideBar()
    {
        activeSideBar = true;

        sideBarSeq = DOTween.Sequence();

        sideBarSeq.Append(sideBar.rectTransform.DOAnchorPosX(-200, aniDuration).SetEase(aniEase));
        sideBarSeq.Join(background.rectTransform.DOAnchorPosX(-100, aniDuration).SetEase(aniEase));
        sideBarSeq.Join(background.transform.DOScale(1.1f, aniDuration).SetEase(aniEase));
        sideBarSeq.Join(arrowEffect.transform.DORotate(new(0, 0, 180), aniDuration).SetEase(aniEase));
        sideBarSeq.Join(logo.DOFade(0, aniDuration).SetEase(Ease.OutQuad));

        sideBarSeq.Play();
    }

    private void DisableSideBar()
    {
        activeSideBar = false;

        sideBarSeq = DOTween.Sequence();

        sideBarSeq.Append(sideBar.rectTransform.DOAnchorPosX(0, aniDuration).SetEase(aniEase));
        sideBarSeq.Join(background.rectTransform.DOAnchorPosX(0, aniDuration).SetEase(aniEase));
        sideBarSeq.Join(background.transform.DOScale(1, aniDuration).SetEase(aniEase));
        sideBarSeq.Join(arrowEffect.transform.DORotate(new(0, 0, 0), aniDuration).SetEase(aniEase));
        sideBarSeq.AppendInterval(0.3f);
        sideBarSeq.Append(logo.DOFade(1, 1f).SetEase(Ease.OutQuad));

        sideBarSeq.Play();
    }
}
