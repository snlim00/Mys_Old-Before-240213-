using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NickNameSceneList
{
    InputScene,
    VerifyScene,
}

public class NicknameScene : MonoBehaviour
{
    [SerializeField] private GameObject inputScene;
    [SerializeField] private GameObject verifyScene;

    [SerializeField] private InputField nicknameInputField;
    [SerializeField] private Button nicknameConfirmButton;

    [SerializeField] private Text verifyInfoText;
    [SerializeField] private Button verifyConfirmButton;
    [SerializeField] private Button verifyCancelButton;

    public void Init()
    {
        OpenScene(NickNameSceneList.InputScene);

        nicknameConfirmButton.onClick.AddListener(() =>
        {
            OpenScene(NickNameSceneList.VerifyScene);
        });

        verifyCancelButton.onClick.AddListener(() =>
        {
            OpenScene(NickNameSceneList.InputScene);
        });

        verifyConfirmButton.onClick.AddListener(() =>
        {
            GameData.saveFile.playerName = nicknameInputField.text;

            SaveManager.Save(GameData.saveFile, RuntimeData.saveFileNumber);

            MysSceneManager.LoadScenarioScene(null, GameConstants.firstScript);
        });
    }

    private void OpenScene(NickNameSceneList scene)
    {
        inputScene.SetActive(false);
        verifyScene.SetActive(false);

        switch(scene)
        {
            case NickNameSceneList.InputScene:
                inputScene.SetActive(true);
                break;

            case NickNameSceneList.VerifyScene:
                verifyScene.SetActive(true);
                InitVerifyScene();
                break;
        }
    }

    private void InitVerifyScene()
    {
        verifyInfoText.text = "정말 [" + nicknameInputField.text + "] 입니까?";
    }
}
