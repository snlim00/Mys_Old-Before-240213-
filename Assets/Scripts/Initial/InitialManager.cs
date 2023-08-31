using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InitialManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameData.saveFile = SaveManager.Load(RuntimeData.saveFileNumber);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            .Subscribe(_ => {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    MysSceneManager.LoadLobbyScene(null);
                }
                else
                {
                    MysSceneManager.LoadEditorLobbyScene(null);
                }
            })
            .AddTo(this);
    }
}
