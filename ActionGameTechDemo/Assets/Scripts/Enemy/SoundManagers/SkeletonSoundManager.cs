using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSoundManager : SoundManager
{
    [Header("Movement")]
    public AudioClip Footstep;
    public AudioClip Runstep;

    [Header("Misc")]
    public AudioClip Idle;
    public AudioClip Swing;
    public AudioClip Attack;
    public AudioClip OnHurt;
    public AudioClip OnStagger;

    public void FootStep()
    {
        PlayOneShot(Footstep);
    }

    public void RunStep()
    {
        PlayOneShot(Runstep);
    }

    public void PlayIdleSound()
    {
        PlayOneShot(Idle);
    }

    public void PlayWeaponSound(int level)
    {
        switch (level)
        {
            default:
            case 0:
                PlayOneShot(Swing);
                return;
            case 1:
                PlayOneShot(Attack);
                return;
        }
    }

    public void PlayHurtSound(int level)
    {
        switch (level)
        {
            default:
            case 0:
                PlayOneShot(OnHurt);
                return;
            case 1:
                PlayOneShot(OnStagger);
                return;
        }
    }
}
