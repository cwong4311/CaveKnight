using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 300;
    public float CurrentHealth = 300;

    public bool IsInvulnerable;
    public bool IsBlocking;

    private float _timeSinceTempInvuln;
    private float _tempInvulnDuration;
    private bool _isTempInvuln;

    public bool IsParrying;
    private float _timeSinceParry;
    private float _tempParryDuration;

    private PlayerController _controller;

    [SerializeField]
    private HealthBar _healthBar;

    public void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _healthBar.SetMaxHealth((int)MaxHealth);
    }

    public void Update()
    {
        if (_isTempInvuln)
        {
            if (Time.time - _timeSinceTempInvuln >= _tempInvulnDuration)
            {
                _isTempInvuln = false;
                IsInvulnerable = false;

                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            }
        }

        if (IsParrying)
        {
            if (Time.time - _timeSinceParry >= _tempParryDuration)
            {
                IsParrying = false;
            }
        }

        IsBlocking = _controller.IsBlocking;
    }

    public bool TakeDamage(float damage)
    {
        if (IsInvulnerable) return false;
        if (IsBlocking) damage *= 0.3f;

        CurrentHealth -= damage;
        _healthBar.SetHealth((int)CurrentHealth);

        _controller.TriggerHitStop(damage, false);

        if (CurrentHealth <= 0.01f)
        {
            _controller.Die();
            IsInvulnerable = true;
        }
        else
        {
            _controller.GetHit(IsBlocking);
            SetTemporaryInvuln(0.4f);
        }

        return true;
    }

    public void SetTemporaryInvuln(float duration)
    {
        IsInvulnerable = true;

        _isTempInvuln = true;
        _timeSinceTempInvuln = Time.time;
        _tempInvulnDuration = duration;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
    }

    public void SetParryState(float duration)
    {
        SetTemporaryInvuln(duration);

        IsParrying = true;
        _timeSinceParry = Time.time;
        _tempParryDuration = duration;
    }
}
