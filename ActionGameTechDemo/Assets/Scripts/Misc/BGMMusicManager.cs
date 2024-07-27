using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMMusicManager : MonoBehaviour
{
    public static Action<string> OnMusicChange;

    private AudioSource _audioSource;

    public AudioClip CaveBGM;
    public AudioClip TownBGM;

    public AudioClip BossNormalBGM;
    public AudioClip BossEnragedBGM;

    public virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public virtual void OnEnable()
    {
        OnMusicChange += OnMusicChanged;

        PlayCageBGM();
    }

    public virtual void OnDisable()
    {
        OnMusicChange -= OnMusicChanged;
    }

    public static void PlayCageBGM()
    {
        OnMusicChange?.Invoke("Cave");
    }

    public static void PlayTownBGM()
    {
        OnMusicChange?.Invoke("Town");
    }

    public static void PlayBossBGM(int intensity)
    {
        var bgm = intensity == 0 ? "BossNormal" : "BossEnraged";
        OnMusicChange?.Invoke(bgm);
    }

    protected void OnMusicChanged(string musicName)
    {
        switch (musicName)
        {
            default:
                // Default is do nothing with the sound
                return;
            case "Cave":
                PlayMusic(CaveBGM);
                return;
            case "Town":
                PlayMusic(TownBGM);
                return;
            case "BossNormal":
                PlayMusic(BossNormalBGM);
                return;
            case "BossEnraged":
                PlayMusic(BossEnragedBGM);
                return;
        }
    }

    private void PlayMusic(AudioClip music)
    {
        if (_audioSource == null) return;
        if (_audioSource.clip == music) return;

        _audioSource.Stop();
        _audioSource.clip = music;
        _audioSource.Play();
    }
}
