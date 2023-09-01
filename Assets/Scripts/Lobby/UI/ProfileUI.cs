using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private Text playerName;
    [SerializeField] private Text moneyAmount;
    [SerializeField] private Text dateAmount;
    [SerializeField] private Image dateProgress;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int date = GameData.saveFile.date;

        dateAmount.text = string.Format("{0:D3}", date);
        dateProgress.fillAmount = date / 9f;
    }
}
