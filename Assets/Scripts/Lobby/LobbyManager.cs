using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgm;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.PlayBGM(bgm);
    }
}
