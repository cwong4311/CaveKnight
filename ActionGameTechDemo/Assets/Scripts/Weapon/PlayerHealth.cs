using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 300;
    public float CurrentHealth = 300;

    public bool IsInvulnerable;

    private float _timeSinceTempInvuln;
    private float _tempInvulnDuration;
    private bool _isTempInvuln;

    private PlayerController _controller;

    public void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public void Update()
    {
        if (_isTempInvuln)
        {
            if (Time.time - _timeSinceTempInvuln >= _tempInvulnDuration)
            {
                _isTempInvuln = false;
                IsInvulnerable = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsInvulnerable) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }

        _controller.GetHit();
    }

    public void Die()
    {
        // Do Nothing Yet
    }

    public void SetTemporaryInvuln(float duration)
    {
        IsInvulnerable = true;

        _isTempInvuln = true;
        _timeSinceTempInvuln = Time.time;
        _tempInvulnDuration = duration;
    }
}
