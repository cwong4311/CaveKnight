using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaurSoundManager : SoundManager
{
    [Header("Movement")]
    public AudioClip Footstep;
    public AudioClip Runstep;

    [Header("Misc")]
    public AudioClip Idle;
    public AudioClip LightSwing;
    public AudioClip MediumSwing;
    public AudioClip Kick;
    public AudioClip Roar;
    public AudioClip Rumble;
    public AudioClip Growl;
    public AudioClip OnHurt;
    public AudioClip Ouch;

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
                PlayOneShot(LightSwing);
                PlayOneShot(Growl);
                return;
            case 1:
                PlayOneShot(MediumSwing);
                PlayOneShot(Rumble);
                return;
            case 2:
                PlayOneShot(Kick);
                PlayOneShot(Roar);
                return;
        }
    }

    public void PlayHurtSound()
    {
        PlayOneShot(OnHurt);
        PlayOneShot(Ouch);
    }
}
