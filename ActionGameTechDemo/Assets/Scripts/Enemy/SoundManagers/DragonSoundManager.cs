using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSoundManager : SoundManager
{
    [Header("Movement")]
    public AudioClip Footstep;
    public AudioClip Runstep;

    [Header("Misc")]
    public AudioClip Bite;
    public AudioClip Tail;
    public AudioClip Fireball;
    public AudioClip Rumble;
    public AudioClip Hurt;
    public AudioClip TakeOff;
    public AudioClip Flap;

    public AudioClip Divebomb;

    public void FootStep()
    {
        PlayOneShot(Footstep);
    }

    public void RunStep()
    {
        PlayOneShot(Runstep);
    }

    public void PlayBiteSound()
    {
        PlayOneShot(Bite);
    }

    public void PlayTailSound()
    {
        PlayOneShot(Tail);
    }

    public void PlayFireballSound()
    {
        PlayOneShot(Fireball);
    }

    public void PlayRumbleSound()
    {
        PlayOneShot(Rumble);
    }

    public void PlayHurtSound()
    {
        PlayOneShot(Hurt);
    }

    public void PlayTakeOffSound()
    {
        PlayOneShot(TakeOff);
    }

    public void PlayFlapWingsSound()
    {
        PlayOneShot(Flap);
    }

    public void PlayDivebombSound()
    {
        PlayOneShot(Divebomb);
    }
}
