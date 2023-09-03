using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public static void PlayBGM(AudioClip audio, bool doFadeIn = true, float fadeDuration = 1)
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            instance.bgmSource.clip = audio;
            instance.bgmSource.Play();
        });

        if(doFadeIn == true)
        {
            seq.AppendCallback(() =>
            {
                instance.bgmSource.volume = 0;
            });
            seq.Append(instance.bgmSource.DOFade(1, fadeDuration));
        }
        else
        {
            seq.AppendCallback(() =>
            {
                instance.bgmSource.volume = 1;
            });
        }

        seq.Play();
    }

    public static void FadeOutBGM(float duration = 1)
    {
        instance.bgmSource.DOFade(0, duration).Play();
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
