using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject exitPopup;
    [SerializeField] private Button okBtn;
    [SerializeField] private Button noBtn;

    private void Start()
    {
        okBtn.onClick.AddListener(() =>
        {
            MysSceneManager.LoadLobbyScene(null);
        });

        noBtn.onClick.AddListener(() =>
        {
            exitPopup.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            exitPopup.SetActive(!exitPopup.activeSelf);
        }
    }
}
