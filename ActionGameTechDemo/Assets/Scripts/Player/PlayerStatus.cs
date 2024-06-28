using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float MaxHealth = 300;
    public float MaxStamina = 120;
    public float MaxMana = 50;
    public float StaminaRegainDelay = 2f;

    private float _currentHealth;
    private float _currentStamina;
    private float _currentMana;
    private float _lastStaminaConsumptionTime = -1;
    
    public bool IsInvulnerable;
    public bool IsBlocking;

    private float _timeSinceTempInvuln;
    private float _tempInvulnDuration;
    private bool _isTempInvuln;

    public bool IsParrying;
    private float _timeSinceParry;
    private float _tempParryDuration;

    private PlayerController _controller;

    [Header("HUD Bars")]
    [SerializeField]
    private StatusBar _healthBar;
    [SerializeField]
    private StatusBar _staminaBar;
    [SerializeField]
    private StatusBar _manaBar;

    public void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    public void OnEnable()
    {
        _currentHealth = MaxHealth;
        _currentStamina = MaxStamina;
        _currentMana = MaxMana;

        _healthBar.SetMaxStatus((int)MaxHealth);
        _staminaBar.SetMaxStatus((int)MaxStamina);
        _manaBar.SetMaxStatus((int)MaxMana);
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

        // If no last stam consumption event, then regain stam slowly, 10stam per second
        if (_lastStaminaConsumptionTime < 0)
        {
            if (_currentStamina < MaxStamina)
                _currentStamina = Mathf.Min(_currentStamina + (Time.deltaTime * 10), MaxStamina);
        }
        // Otherwise, wait for a delay before allowing stam to regain after last consumption
        else if (Time.time - _lastStaminaConsumptionTime >= StaminaRegainDelay)
        {
            _lastStaminaConsumptionTime = -1;
        }

        // Regain mana at a rate of 1mp every second
        if (_currentMana < MaxMana)
        {
            _currentMana = Mathf.Min(_currentMana + (Time.deltaTime), MaxMana);
        }

        IsBlocking = _controller.IsBlocking;

        _healthBar.SetStatus((int)_currentHealth);
        _staminaBar.SetStatus((int)_currentStamina);
        _manaBar.SetStatus((int)_currentMana);
    }

    public bool TakeDamage(float damage)
    {
        if (IsInvulnerable) return false;
        if (IsBlocking) damage *= 0.3f;

        _currentHealth -= damage;

        _controller.TriggerHitStop(damage, false);

        if (_currentHealth <= 0.01f)
        {
            _controller.Die();
            IsInvulnerable = true;
        }
        else
        {
            _controller.GetHit(IsBlocking);
            SetTemporaryInvuln(duration: 0.4f, keepCollision: false);
        }

        return true;
    }

    public void SetTemporaryInvuln(float duration, bool keepCollision)
    {
        IsInvulnerable = true;

        _isTempInvuln = true;
        _timeSinceTempInvuln = Time.time;
        _tempInvulnDuration = duration;

        if (!keepCollision)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        }
    }

    public void SetParryState(float duration)
    {
        SetTemporaryInvuln(duration: duration, keepCollision: true);

        IsParrying = true;
        _timeSinceParry = Time.time;
        _tempParryDuration = duration;
    }

    /// <summary>
    /// Restores hp
    /// </summary>
    /// <param name="amountConsumed"></param>
    /// <returns></returns>
    public bool RestoreHealth(float amountRestored)
    {
        if (_currentHealth < MaxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + amountRestored, MaxHealth);
        }

        return true;
    }

    /// <summary>
    /// Consumes stamina. If return true, it means this operation is a success
    /// If false, it means no stamina was consumed, as there wasn't enough
    /// </summary>
    /// <param name="amountConsumed"></param>
    /// <returns></returns>
    public bool ConsumeStamina(float amountConsumed)
    {
        if (_currentStamina >= amountConsumed)
        {
            _currentStamina -= amountConsumed;
            _lastStaminaConsumptionTime = Time.time;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Consumes mana. If return true, it means this operation is a success
    /// If false, it means no mana was consumed, as there wasn't enough
    /// </summary>
    /// <param name="amountConsumed"></param>
    /// <returns></returns>
    public bool ConsumeMana(float amountConsumed)
    {
        if (_currentMana >= amountConsumed)
        {
            _currentMana -= amountConsumed;
            return true;
        }

        return false;
    }
}
