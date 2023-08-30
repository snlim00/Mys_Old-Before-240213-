using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableTooltip : Singleton<VariableTooltip>
{
    [SerializeField] private Text textShape;
    [SerializeField] private Text text;

    public void ShowTooltip(string text)
    {
        textShape.text = text;
        this.text.text = text;

        textShape.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        textShape.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(textShape.gameObject.activeSelf == true)
        {
            transform.position = Input.mousePosition;
        }
    }
}
