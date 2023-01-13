using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScriptList : MonoBehaviour
{
    public ScriptObject script;

    public Text scriptText;
    public GameObject selectHighlight;
    public Button selectButton;

    /*
     * 図楕拭 毒楽? 句随猿
     * 努什闘櫛
     * 戚坤闘
     * 左食爽壱.....
     * 焼たたたたたたたたたたたたた
     * 企亜軒斗びじけ
     * 
     * */

    public void SetText(string text)
    {
        scriptText.text = text;
    }

    public void SetHighlight(bool highlight)
    {
        selectHighlight.SetActive(highlight);
    }

    public void SetButtonCallback(UnityAction callback)
    {
        selectButton.onClick.AddListener(callback);
    }
}
