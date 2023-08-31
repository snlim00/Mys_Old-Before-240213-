using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkMainScene : PhoneScene
{
    [SerializeField] private List<TalkProfileObject> profileList;

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
}
