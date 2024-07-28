using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 10000;
    public float CurrentHealth = 10000;

    public float HealthPercentage => CurrentHealth / MaxHealth;

    public bool IsInvulnerable;
    private EnemyController _controller;

    [SerializeField]
    private BossHealthBar _healthBar;

    [SerializeField]
    private BossHealthBar _floatingBarPF;

    public bool isFloatingBar;

    public float FloatingBarYOffset;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _controller = GetComponent<EnemyController>();

        if (!isFloatingBar)
        {
            _healthBar?.SetMaxStatus((int)MaxHealth);
            _healthBar?.HideHealthBar();
        }
        else
        {
            var hpCanvas = FindObjectsOfType<Canvas>().FirstOrDefault(e => e.renderMode == RenderMode.WorldSpace);
            if (hpCanvas == null || _floatingBarPF == null) return;

            _healthBar = Instantiate(_floatingBarPF, hpCanvas.transform);
            if (_healthBar == null) return;

            _healthBar.FollowTarget = this.transform;
            _healthBar.YOffset = FloatingBarYOffset;
            _healthBar.SetMaxStatus((int)MaxHealth);
            _healthBar?.HideHealthBar();
        }
    }

    public void OnDestroy()
    {
        if (isFloatingBar && _healthBar != null)
        {
            Destroy(_healthBar.gameObject);
        }
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

        _healthBar?.SetStatus((int)CurrentHealth);
        _controller.GetHit(damage);

        if (isFloatingBar)
        {
            _healthBar?.ShowHealthBar();
        }

        return true;
    }

    public void Die()
    {
        _controller.Die();
    }

    public void SetInvuln()
    {
        IsInvulnerable = true;
    }

    public void RemoveInvuln()
    {
        IsInvulnerable = false;
    }

    public BossHealthBar GetHealthHUD()
    {
        return _healthBar;
    }
}
