using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogObject : MonoBehaviour
{
    private EventManager eventMgr;

    private RectTransform rectTransform;
    public Image image;

    public string objName = string.Empty;

    public float headPosition { get; private set; } = 0;

    public float position { get; private set; } = 0;

    public Vector2 canvasSize = Vector2.zero;

    private void Awake()
    {
        eventMgr = FindObjectOfType<EventManager>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(Sprite sprite, string name, float position, float headPosition)
    {
        this.objName = name;
        SetSprite(sprite);
        SetPosition(position);
        SetHeadPosition(headPosition);
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;

        image.SetNativeSize();

        var sizeDelta = image.rectTransform.sizeDelta;

        sizeDelta.x = (sizeDelta.x * 450) / sizeDelta.y; //450은 캔버스의 높이임. 이후 리터럴이 아닌 값을 구해서 사용하도로 변경할 것.

        sizeDelta.y = 450;

        image.rectTransform.sizeDelta = sizeDelta;
    }

    public void SetPosition(float position)
    {
        transform.localPosition = GetPosition(position);
    }

    public void SetHeadPosition(float headPosition)
    { 
        this.headPosition = headPosition; 
    }

    public Tween MoveObject(float position, float duration, Ease ease)
    {
        return transform.DOLocalMove(GetPosition(position), duration).SetEase(ease);
    }

    public void SetScale(float scale, bool fittingTop)
    {
        transform.localScale = Vector2.one * scale;

        if(fittingTop == true)
        {
            var pos = transform.localPosition;

            pos.y = ((rectTransform.sizeDelta.y * transform.localScale.x) - rectTransform.sizeDelta.y) / -2f;

            transform.localPosition = pos;
        }
        else
        {
            var pos = transform.localPosition;

            pos.y = 0;

            transform.localPosition = pos;
        }
    }

    public Vector3 GetPosition(float position)
    {
        //800은 캔버스의 너비임. 이후 리터럴이 아닌 값을 구해서 사용하도로 변경할 것.
        return new(((800f / 7) * position) - (800f / 2), 0, 0);
    }
}