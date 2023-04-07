using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class VariableTooltip : MonoBehaviour
{
    public static VariableTooltip instance;

    [SerializeField] private Text text;
    [SerializeField] private RectTransform panel;

    private RectTransform textRect;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        gameObject.SetActive(false);

        textRect = text.GetComponent<RectTransform>();
    }

    IDisposable stream = null;

    public void ShowTooltip(string explain)
    {
        gameObject.SetActive(true);
        text.text = explain;

        stream = Observable.EveryUpdate()
            .Subscribe(_ => this.transform.position = Input.mousePosition);

        Observable.TimerFrame(1)
            .Subscribe(_ =>
            {
                textRect.localPosition = textRect.sizeDelta / 2 + new Vector2(5, 5);

                panel.sizeDelta = textRect.sizeDelta + new Vector2(7, 10);
                panel.localPosition = textRect.localPosition;
            });
    }

    public void HideTooltip()
    {
        if(stream != null)
        {
            stream.Dispose();
        }

        gameObject.SetActive(false);
    }
}
