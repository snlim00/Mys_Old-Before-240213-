using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public static SoundManager instance;

    [SerializeField] private AudioSource bgmSource;

    private static int sfxSourceIdx = 0;
    [SerializeField] private List<AudioSource> sfxSource;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        instance = SoundManager.Instance;
    }

    public static void PlayBGM(AudioClip audio)
    {
        instance.bgmSource.clip = audio;
        instance.bgmSource.Play();
    }

    public static void PlaySFX(AudioClip audio)
    {
        instance.sfxSource[sfxSourceIdx].clip = audio;
        instance.sfxSource[sfxSourceIdx].Play();

        sfxSourceIdx++;

        if(sfxSourceIdx >= instance.sfxSource.Count)
        {
            sfxSourceIdx = 0;
        }
    }
}
