using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MouseEffect : MonoBehaviour
{
    [SerializeField] private Image img;

    private bool isActive = false;

    private Tween effectTween;

    private RectTransform rectTransform;

    private Vector2 defaultAnchorPos;

    private Tween floatTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultAnchorPos = rectTransform.anchoredPosition;
    }

    public void ActiveMouseEffect(bool doActive)
    {
        if(effectTween?.IsPlaying() == true)
        {
            effectTween.Kill();
            effectTween = null;
        }

        if(floatTween?.IsPlaying() == true)
        {
            floatTween.Kill(true);
            floatTween = null;
            rectTransform.anchoredPosition = new(rectTransform.anchoredPosition.x, defaultAnchorPos.y);
        }

        float alpha = doActive == true ? 1 : 0;
        float duration = doActive == true ? 0.4f : 0.1f;
        isActive = doActive;

        effectTween = img.DOFade(alpha, duration).Play();
        floatTween = rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - 5, 1).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Play();
    }
}
