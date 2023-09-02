using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public static SoundManager instance;

    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        instance = SoundManager.Instance;
    }

    private void Update()
    {
        
    }

    public static void PlayBGM(AudioClip audio)
    {
        instance.bgmSource.clip = audio;
        instance.bgmSource.Play();
    }
}
