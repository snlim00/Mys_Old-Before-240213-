using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private NicknameScene nicknameScene;

    // Start is called before the first frame update
    void Start()
    {
        if (GameData.saveFile.chapterData[CharacterInfo.Public] == 0)
        {
            nicknameScene.gameObject.SetActive(true);
            nicknameScene.Init();
        }
    }
}
