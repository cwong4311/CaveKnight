using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamager : MonoBehaviour
{
    private Collider _damageCollider;

    private float _damageDealt = 0f;
    private float _tickDamage = 0f;
    private Coroutine _delayedWeaponActive = null;

    private bool _targetEnemy;

    public void Awake()
    {
        _damageCollider = GetComponent<Collider>();
        _damageCollider.gameObject.SetActive(true);
        _damageCollider.isTrigger = true;
        _damageCollider.enabled = false;
    }

    // TO DO: Damage Once
    public void OnTriggerEnter(Collider collision)
    {
        if (IsDamageable(collision) && _damageDealt > 0.01f)
        {
            if (_targetEnemy && collision.gameObject.TryGetComponent<EnemyHealth>(out var enemy))
            {
                enemy.TakeDamage(_damageDealt);
            }
            else if (!_targetEnemy && collision.gameObject.TryGetComponent<PlayerHealth>(out var player))
            {
                player.TakeDamage(_damageDealt);
            }
            // Else maybe a destructable. Handle later
        }
    }

    public void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Damageable" && _tickDamage > 0.01f)
        {

        }
    }

    public void OnTriggerExit(Collider collision)
    {

    }

    public void SetWeaponTarget(bool targetEnemy)
    {
        _targetEnemy = targetEnemy;
    }

    public void ActivateWeapon(float damage)
    {
        if (_delayedWeaponActive != null)
        {
            StopCoroutine(_delayedWeaponActive);
            _delayedWeaponActive = null;
        }

        _damageDealt = damage;
        _damageCollider.enabled = true;
    }

    public void ActivateWeapon(float damage, float delay)
    {
        if (_delayedWeaponActive != null)
        {
            StopCoroutine(_delayedWeaponActive);
            _delayedWeaponActive = null;
        }

        _delayedWeaponActive = StartCoroutine(DelayedWeaponActive(damage, delay));
    }

    private IEnumerator DelayedWeaponActive(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);

        _damageDealt = damage;
        _damageCollider.enabled = true;
        _delayedWeaponActive = null;
    }

    public void DeactivateWeapon()
    {
        if (_delayedWeaponActive != null)
        {
            StopCoroutine(_delayedWeaponActive);
            _delayedWeaponActive = null;
        }

        _damageCollider.enabled = false;
    }

    public bool IsDamageable(Collider collision)
    {
        if (collision.tag == "Player" || collision.tag == "Enemy" || collision.tag == "Destructable")
        {
            return true;
        }

        return false;
    }
}
