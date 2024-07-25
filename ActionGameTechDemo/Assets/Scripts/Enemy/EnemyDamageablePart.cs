using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Damageable part of an enemy. Allows customised lockon AND customised damageTaken behaviour
/// </summary>
public class EnemyDamageablePart : MonoBehaviour, ILockOnAbleObject
{
    public Transform LockOnRoot;

    public EnemyHealth EnemyHealth;

    public Transform LockOnTarget => LockOnRoot ?? transform;

    public bool TakeDamage(float damage)
    {
        if (EnemyHealth != null)
        {
            return EnemyHealth.TakeDamage(damage);
        }

        return false;
    }
}
