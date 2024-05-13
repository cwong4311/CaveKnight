using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageablePart : MonoBehaviour
{
    public EnemyHealth EnemyHealth;

    public void TakeDamage(float damage)
    {
        if (EnemyHealth != null)
        {
            EnemyHealth.TakeDamage(damage);
        }
    }
}
