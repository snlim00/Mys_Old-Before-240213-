using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
using UniRx;

public class MysSceneManager : Singleton<MysSceneManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static void LoadLobbyScene(Action cb)
    {
        SceneManager.LoadScene("Lobby");

        cb?.Invoke();
    }

    public static void LoadEditorLobbyScene(Action cb)
    {
        SceneManager.LoadScene("EditorLobby");

        cb?.Invoke();
    }

    public static void LoadScenarioScene(Action cb, int? scriptGroupId = null)
    {
        MysSceneManager.LoadScene("ScenarioScene", () => {
            cb?.Invoke();
            
            if(scriptGroupId != null)
            {
                EditorManager.Instance.gameObject.SetActive(false);
                DialogManager.Instance.StartDialog(scriptGroupId ?? 0);
            }
        });
    }

    public static void LoadScene(string scene, Action cb)
    {
        SceneManager.LoadScene(scene);

        Observable.TimerFrame(1)
            .Subscribe(_ => cb?.Invoke());
    }
}