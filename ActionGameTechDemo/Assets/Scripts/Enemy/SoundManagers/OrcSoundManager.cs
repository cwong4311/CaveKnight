using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcSoundManager : SoundManager
{
    [Header("Movement")]
    public AudioClip Footstep;
    public AudioClip Runstep;

    [Header("Misc")]
    public AudioClip LightSwing;
    public AudioClip MediumSwing;
    public AudioClip HeavySwing;
    public AudioClip OnHurt;
    public AudioClip Dodge;

    public void FootStep()
    {
        PlayOneShot(Footstep);
    }

    public void RunStep()
    {
        PlayOneShot(Runstep);
    }

    public void PlayWeaponSound(int level)
    {
        switch (level)
        {
            default:
            case 0:
                PlayOneShot(LightSwing);
                return;
            case 1:
                PlayOneShot(MediumSwing);
                return;
            case 2:
                PlayOneShot(HeavySwing);
                return;
        }
    }

    public void PlayHurtSound()
    {
        PlayOneShot(OnHurt);
    }

    public void PlayDodgeSound()
    {
        PlayOneShot(Dodge);
    }
}
