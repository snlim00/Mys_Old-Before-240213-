using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkMainScene : PhoneScene
{
    [SerializeField] private List<TalkProfileObject> profileList;

    [SerializeField] private NotiableObject jihyaeProfile;
    [SerializeField] private NotiableObject yunhaProfile;
    [SerializeField] private NotiableObject seeunProfile;

    protected override void Start()
    {
        base.Start();

        foreach (var profile in profileList)
        {
            profile.profileImage.onClick.AddListener(() =>
            {
                phoneMgr.OpenScene(PhoneSceneList.TalkProfile);

                phoneMgr.talkProfileScene.Init(
                    profile.characterName, 
                    profile.profileName.text, 
                    profile.message.text, 
                    profile.profileMusic.text, 
                    profile.profileImage.image.sprite);
            });
        }
    }

    protected override void OnHomeButtonClick()
    {
        base.OnHomeButtonClick();

        phoneMgr.OpenScene(PhoneSceneList.Home);
    }

    protected override void OnBackButtonClick()
    {
        base.OnBackButtonClick();

        phoneMgr.OpenScene(PhoneSceneList.Home);
    }

    public override void OnOpenScene()
    {
        base.OnOpenScene();

        var chapterData = GameData.saveFile.chapterData;

        if(chapterData[CharacterInfo.Jihyae] < 3)
        {
            jihyaeProfile.ShowNoti(true);
        }

        if(chapterData[CharacterInfo.Yunha] < 3)
        {
            yunhaProfile.ShowNoti(true);
        }

        if(chapterData[CharacterInfo.Seeun] < 3)
        {
            seeunProfile.ShowNoti(true);
        }
    }
}
