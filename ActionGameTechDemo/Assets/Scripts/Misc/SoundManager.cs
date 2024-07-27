using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundManager : MonoBehaviour
{
    private AudioSource _audioSource;

    public virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void PlayOneShot(AudioClip audioClip)
    {
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(audioClip);
        }
    }
}
