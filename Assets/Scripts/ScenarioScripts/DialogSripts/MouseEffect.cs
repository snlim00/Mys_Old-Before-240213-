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

    public void ActiveMouseEffect(bool doActive)
    {
        if(effectTween?.IsPlaying() == true)
        {
            effectTween.Kill();
            effectTween = null;
        }

        float alpha = doActive == true ? 1 : 0;
        float duration = doActive == true ? 0.4f : 0.1f;
        isActive = doActive;

        effectTween = img.DOFade(alpha, duration).Play();
    }
}
