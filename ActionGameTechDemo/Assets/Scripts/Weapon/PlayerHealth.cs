using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 1000;
    public float CurrentHealth = 1000;

    public bool IsInvulnerable;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsInvulnerable) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Do Nothing Yet
    }
}
