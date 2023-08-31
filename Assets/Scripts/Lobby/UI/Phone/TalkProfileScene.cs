using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkProfileScene : PhoneScene
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button storyBtn;
    [SerializeField] private Image profileImage;
    [SerializeField] private Text profileName;
    [SerializeField] private Text message;
    [SerializeField] private Image profileMusicImage;
    [SerializeField] private Text profileMusic;

    [SerializeField] private GameObject storyPopup;
    [SerializeField] private Button storyStartBtn;

    private string targetCharacterName;

    protected override void Start()
    {
        base.Start();

        storyBtn.onClick.AddListener(() =>
        {
            storyPopup.SetActive(true);
        });

        closeBtn.onClick.AddListener(() =>
        {
            phoneMgr.OpenScene(PhoneSceneList.TalkMain);
        });
    }

    public void Init(string characterName, string profileName, string message, string profileMusic, Sprite sprite)
    {
        targetCharacterName = characterName;
        this.profileName.text = profileName;
        this.message.text = message;
        this.profileMusic.text = profileMusic;
        profileImage.sprite = sprite;

        if(string.IsNullOrWhiteSpace(targetCharacterName))
        {
            storyBtn.interactable = false;
        }

        if(string.IsNullOrWhiteSpace(profileMusic))
        {
            this.profileMusicImage.gameObject.SetActive(false);
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

        phoneMgr.OpenScene(PhoneSceneList.TalkMain);
    }
}
