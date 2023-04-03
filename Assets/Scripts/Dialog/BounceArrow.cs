using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BounceArrow : MonoBehaviour
{
    private Image image;

    private Tween tween = null;
    private bool isEnabled = false;

    private Vector3 startPos;

    private void Awake()
    {
        image = GetComponent<Image>();
        startPos = transform.position;
    }

    public void SetEnable(bool enable)
    {
        if (enable == true && isEnabled == false)
        {
            image.DOFade(1, 0.2f);

            transform.position = startPos;

            float pos = image.transform.localPosition.y - 7;
            tween = image.transform.DOLocalMoveY(pos, 0.55f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            isEnabled = true;
        }
        else if (enable == false && isEnabled == true)
        {
            tween?.Kill();

            image.DOFade(0, 0.2f);

            isEnabled = false;
        }
    }
}
