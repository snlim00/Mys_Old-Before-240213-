using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InitialManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgm;

    // Start is called before the first frame update
    void Start()
    {
        GameData.saveFile = SaveManager.Load(RuntimeData.saveFileNumber);

        SoundManager.PlayBGM(bgm);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    MysSceneManager.LoadEditorLobbyScene(null);
                }
            })
            .AddTo(this);

        Screen.SetResolution(1920, 1080, true);
    }
}
