using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using static UnityEditor.PlayerSettings;

public class MouseEffect : MonoBehaviour
{
    [SerializeField] private Image img;

    private TextManager textMgr;

    private bool isActive = false;

    private Tween effectTween;

    private RectTransform rectTransform;

    private Vector2 defaultAnchorPos;

    private Tween floatTween;

    public const float mouseEffectInterval = 11;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        defaultAnchorPos = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        textMgr = FindObjectOfType<TextManager>();
    }

    public void ActiveMouseEffect(bool doActive, float? pos)
    {
        if (effectTween?.IsPlaying() == true)
        {
            effectTween.Kill();
            effectTween = null;
        }

        if (floatTween?.IsPlaying() == true)
        {
            floatTween.Kill(true);
            floatTween = null;
            rectTransform.anchoredPosition = new(rectTransform.anchoredPosition.x, defaultAnchorPos.y);
        }

        if (doActive == true)
        {
            rectTransform.localPosition = new((pos ?? 0) + mouseEffectInterval, rectTransform.localPosition.y);
        }

        float alpha = doActive == true ? 1 : 0;
        float duration = doActive == true ? 0.4f : 0.1f;
        isActive = doActive;

        effectTween = img.DOFade(alpha, duration).Play();
        floatTween = rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - 4, 0.7f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Play();
    }
}
