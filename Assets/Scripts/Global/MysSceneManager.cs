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

    public static void LoadDialogScene(Action cb)
    {
        MysSceneManager.LoadScene("ScenarioScene", cb);
    }

    public static void LoadScene(string scene, Action cb)
    {
        SceneManager.LoadScene(scene);

        Observable.TimerFrame(1)
            .Subscribe(_ => cb?.Invoke());
    }
}