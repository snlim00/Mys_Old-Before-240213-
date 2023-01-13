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
     * 왼쪽에 판넬? 띄울까
     * 텍스트랑
     * 이벤트
     * 보여주고.....
     * 아ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
     * 대가리터ㅣㅈㅁ
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
