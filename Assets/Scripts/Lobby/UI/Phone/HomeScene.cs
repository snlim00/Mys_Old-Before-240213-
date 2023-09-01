using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScene : PhoneScene
{
    [SerializeField] private Button talkBtn;
    [SerializeField] private Button exitBtn;

    [SerializeField] private NotiableObject talkApp; 

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

    public override void OnOpenScene()
    {
        base.OnOpenScene();

        foreach(var chapterData in GameData.saveFile.chapterData) //DEMO : ������ é�Ͱ� 2 ������ ĳ���Ͱ� �ϳ��� �ִٸ� ��Ƽ ����
        {
            if(chapterData.Value < 3)
            {
                talkApp.ShowNoti(true);
                return;
            }
        }

        talkApp.ShowNoti(false);
    }
}
