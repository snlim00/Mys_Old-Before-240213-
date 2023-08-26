using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtilityExtension
{
    public static void SetAlpha(this MaskableGraphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;

        graphic.color = color;
    }

    public static void SetButtonText(this Button btn, string text)
    {
        if (btn == null) return;

        var label = btn.transform.GetComponentInChildren<Text>();

        if (label == null) return;

        label.text = text;
    }
}
