using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InitialManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameData.saveFile = SaveFile.CreateNewSaveFile();

        Observable.EveryUpdate()
            .Where(_ => Input.anyKeyDown || Input.GetMouseButtonDown(0))
            .Subscribe(_ => MysSceneManager.LoadLobbyScene(null))
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
