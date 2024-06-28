using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxStatus(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetStatus(int currentHealth)
    {
        slider.value = currentHealth;
    }

    public float GetStatus()
    {
        return slider.value;
    }
}
