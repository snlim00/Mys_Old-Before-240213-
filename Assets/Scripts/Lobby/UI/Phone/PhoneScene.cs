using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneScene : MonoBehaviour
{
    protected PhoneManager phoneMgr;

    [SerializeField] protected Button etcButton;
    [SerializeField] protected Button homeButton;
    [SerializeField] protected Button backButton;

    protected virtual void Start()
    {
        phoneMgr = PhoneManager.Instance;

        etcButton.onClick.AddListener(OnEtcButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    protected virtual void OnEtcButtonClick()
    {

    }

    protected virtual void OnHomeButtonClick()
    {

    }

    protected virtual void OnBackButtonClick()
    {

    }
}
