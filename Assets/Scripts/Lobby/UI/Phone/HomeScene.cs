using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScene : PhoneScene
{
    [SerializeField] private Button talkBtn;
    [SerializeField] private Button exitBtn;

    protected override void Start()
    {
        base.Start();

        talkBtn.onClick.AddListener(OnTalkButtonClick);
        exitBtn.onClick.AddListener(OnExitButtonClick);
    }

    private void OnTalkButtonClick()
    {
        phoneMgr.OpenScene(PhoneSceneList.TalkMain);
    }

    private void OnExitButtonClick()
    {
        phoneMgr.OnPutInPhoneButtonClick();
    }
}
