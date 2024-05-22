using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 10000;
    public float CurrentHealth = 10000;

    public float HealthPercentage => CurrentHealth / MaxHealth;

    public bool IsInvulnerable;
    private EnemyController _controller;

    [SerializeField]
    private HealthBar _healthBar;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth / 2 + 1;
        _controller = GetComponent<EnemyController>();

        _healthBar?.SetMaxHealth((int)MaxHealth);
    }

    /// <summary>
    /// Returns whether damage was taken or not
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool TakeDamage(float damage)
    {
        if (IsInvulnerable) return false;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }

        _healthBar?.SetHealth((int)CurrentHealth);
        _controller.GetHit(damage);

        return true;
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
