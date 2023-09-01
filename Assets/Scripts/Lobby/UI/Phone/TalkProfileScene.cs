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
    [SerializeField] private Text storyTitle;
    [SerializeField] private Text storyCondition;
    [SerializeField] private Button storyStartBtn;

    [SerializeField] private Sprite defaultStoryStartSprite;
    [SerializeField] private Sprite activeStoryStartSprite;

    private string targetCharacterName;

    private int scenarioId;

    protected override void Start()
    {
        base.Start();

        storyBtn.onClick.AddListener(() =>
        {
            storyPopup.SetActive(!storyPopup.activeSelf);
        });

        closeBtn.onClick.AddListener(() =>
        {
            phoneMgr.OpenScene(PhoneSceneList.TalkMain);
        });

        storyStartBtn.onClick.AddListener(() =>
        {
            MysSceneManager.LoadScenarioScene(null, scenarioId);
        });
    }

    public void Init(string characterName, string profileName, string message, string profileMusic, Sprite sprite)
    {
        targetCharacterName = characterName;
        this.profileName.text = profileName;
        this.message.text = message;
        this.profileMusic.text = profileMusic;
        profileImage.sprite = sprite;

        if (string.IsNullOrWhiteSpace(profileMusic))
        {
            this.profileMusicImage.gameObject.SetActive(false);
        }

        if (string.IsNullOrWhiteSpace(targetCharacterName))
        {
            storyBtn.image.sprite = defaultStoryStartSprite;
            storyBtn.interactable = false;
        }
        else
        {
            storyStartBtn.interactable = true;

            int lastClearedChapter = GameData.saveFile.chapterData[characterName];

            if(lastClearedChapter >= 3) //DEMO : 3 챕터까지만 존재함.
            {
                storyTitle.text = "[ 개발중 ]";
                storyCondition.text = "";
                storyStartBtn.interactable = false;
                storyBtn.image.sprite = defaultStoryStartSprite;

                return;
            }

            storyBtn.image.sprite = activeStoryStartSprite;

            int nextScenarioId = ScriptManager.GetNextScenarioId(characterName, lastClearedChapter);

            RuntimeData.scriptMgr = CSVReader.ReadScript(nextScenarioId);

            scenarioId = nextScenarioId;

            storyTitle.text = RuntimeData.scriptMgr.title;
            storyCondition.text = RuntimeData.scriptMgr.explain;
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

    public override void OnCloseScene()
    {
        base.OnCloseScene();

        storyPopup.SetActive(false);
    }
}
