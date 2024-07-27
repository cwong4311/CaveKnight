using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specifically Melee Weapons. Contains OnParried logic
/// </summary>
public class WeaponDamager : MonoBehaviour
{
    public GameObject OnHitEffect;

    private Collider _damageCollider;

    private float _damageDealt = 0f;
    private float _tickDamage = 0f;
    private Coroutine _delayedWeaponActive = null;

    // true is Enemy, false is Player
    private bool _targetIsEnemy;

    private CharacterManager _myCharacter;
    private List<object> _alreadyHitTargets = new List<object>();

    public bool IsActive => _damageCollider.enabled;

    public void Awake()
    {
        _myCharacter = GetComponentInParent<CharacterManager>();
        _damageCollider = GetComponent<Collider>();
        _damageCollider.gameObject.SetActive(true);
        _damageCollider.isTrigger = true;
        _damageCollider.enabled = false;
    }

    // TO DO: Damage Once
    public void OnTriggerEnter(Collider collision)
    {
        if (IsDamageable(collision) == false || _damageDealt < 0.01f) return;

        if (_alreadyHitTargets.Contains(collision)) return;

        bool hasDealtDamage = false;
        if (_targetIsEnemy)
        {
            if (collision.gameObject.TryGetComponent<EnemyHealth>(out var enemy))
            {
                hasDealtDamage = enemy.TakeDamage(_damageDealt);
            }
            else if (collision.gameObject.TryGetComponent<EnemyDamageablePart>(out var enemyPart))
            {
                hasDealtDamage = enemyPart.TakeDamage(_damageDealt);
            }
        }
        else if (!_targetIsEnemy && collision.gameObject.TryGetComponent<PlayerStatus>(out var player))
        {
            if (player.IsParrying)
            {
                _myCharacter?.GetComponent<EnemyController>()?.ForceGetHit();
                player.ProcParryActive(collision);
            }

            hasDealtDamage = player.TakeDamage(_damageDealt);
        }
        // Else maybe a destructable. Handle later

        if (hasDealtDamage)
        {
            _alreadyHitTargets.Add(collision);
            _myCharacter.TriggerHitStop(_damageDealt, true);

            var pointOfCollision = collision.ClosestPoint(transform.position);
            if (OnHitEffect != null)
            {
                Instantiate(OnHitEffect, pointOfCollision, Quaternion.identity);
            };
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
        _targetIsEnemy = targetEnemy;
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
        _alreadyHitTargets.Clear();
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
        _alreadyHitTargets.Clear();
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
