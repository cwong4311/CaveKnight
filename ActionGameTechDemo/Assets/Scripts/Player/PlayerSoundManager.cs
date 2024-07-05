using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : SoundManager
{
    [Header("Movement")]
    public AudioClip Footstep;
    public AudioClip Runstep;

    [Header("Weapon")]
    public AudioClip LightSwing;
    public AudioClip MediumSwing;
    public AudioClip HeavySwing;
    public AudioClip Smash;

    [Header("Spell")]
    public AudioClip Heal;

    [Header("Misc")]
    public AudioClip OnHurt;
    public AudioClip OnBlock;
    public AudioClip OnParry;

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
            case 3:
                PlayOneShot(Smash);
                return;
        }
    }

    public void Hurt()
    {
        PlayOneShot(OnHurt);
    }

    public void Block()
    {
        PlayOneShot(OnBlock);
    }

    public void Parry()
    {
        PlayOneShot(OnParry);
    }

    public void PlayHealSound()
    {
        PlayOneShot(Heal);
    }
}
