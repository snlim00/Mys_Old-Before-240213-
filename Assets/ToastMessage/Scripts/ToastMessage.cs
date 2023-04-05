using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class ToastMessage : MonoBehaviour
{
    [SerializeField] private Text text;

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();

        transform.SetParent(ToastManager.instance.content.transform);
        transform.localScale = Vector3.one;
        transform.position = new Vector2(Screen.width / 2, Screen.height * 0.85f);
    }

    public void Print(string message, float duration = 2f)
    {
        text.text = message;

        img.SetAlpha(0);
        text.SetAlpha(0);

        Sequence fadeIn = DOTween.Sequence();
        Sequence fadeOut = DOTween.Sequence();

        fadeIn.Append(img.DOFade(0.3f, 0.2f));
        fadeIn.Insert(0, text.DOFade(1f, 0.2f));

        fadeIn.AppendInterval(duration);
        fadeIn.AppendCallback(() => fadeOut.Play());

        fadeOut.Append(img.DOFade(0, 0.2f));
        fadeOut.Insert(0, text.DOFade(0, 0.2f));
        fadeOut.AppendCallback(() => Destroy(this.gameObject));

        fadeIn.Play();
    }
}
