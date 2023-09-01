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
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    MysSceneManager.LoadEditorLobbyScene(null);
                }
                else
                {
                    MysSceneManager.LoadLobbyScene(null);
                }
            })
            .AddTo(this);
    }
}
