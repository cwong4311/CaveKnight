using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 10000;
    public float CurrentHealth = 10000;

    public bool IsInvulnerable;
    private EnemyController _controller;

    [SerializeField]
    private HealthBar _healthBar;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _controller = GetComponent<EnemyController>();

        _healthBar.SetMaxHealth((int)MaxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsInvulnerable) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }

        _healthBar.SetHealth((int)CurrentHealth);
        _controller.GetHit(damage);
    }

    public void Die()
    {
        // Do Nothing Yet
    }

    public void SetInvuln()
    {
        IsInvulnerable = true;
    }

    public void RemoveInvuln()
    {
        IsInvulnerable = false;
    }
}
