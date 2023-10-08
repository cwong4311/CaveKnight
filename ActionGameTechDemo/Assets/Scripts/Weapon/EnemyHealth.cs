using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 1000;
    public float CurrentHealth = 1000;

    public bool IsInvulnerable;
    private EnemyController _controller;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _controller = GetComponent<EnemyController>();
    }

    public void TakeDamage(float damage)
    {
        if (IsInvulnerable) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }

        _controller.GetHit();
    }

    public void Die()
    {
        // Do Nothing Yet
    }
}
