using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;

public class MysSceneManager : Singleton<MysSceneManager>
{
    private MysSceneManager instance;

    [SerializeField] private Image fadeObj;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        instance = MysSceneManager.Instance;
    }

    public static void LoadInitialScene(Action cb)
    {
        LoadScene("InitialScene", cb);
    }

    public static void LoadLobbyScene(Action cb)
    {
        LoadScene("Lobby", cb);
    }

    public static void LoadEditorLobbyScene(Action cb)
    {
        LoadScene("EditorLobby", cb);
    }

    public static void LoadScenarioScene(Action cb, int? scriptGroupId = null)
    {
        LoadScene("ScenarioScene", () => {
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
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            SoundManager.FadeOutBGM();
        });

        seq.Append(Instance.fadeObj.DOFade(1, 1).SetEase(Ease.OutQuad));

        seq.AppendCallback(() =>
        {
            SceneManager.LoadScene(scene);

            Observable.TimerFrame(1)
                .Subscribe(_ => cb?.Invoke());
        });

        seq.Append(Instance.fadeObj.DOFade(0, 1).SetEase(Ease.OutQuad));

        seq.Play();
    }
}