using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball: MonoBehaviour
{
    public Transform FireballLocation;
    public GameObject FireballPF;

    public void SpawnFireball(Transform target, bool isHoming)
    {
        var fireballGO = Instantiate(FireballPF, FireballLocation.position, Quaternion.identity);
        var projectile = fireballGO.GetComponent<Projectile>();

        if (isHoming)
        {
            projectile.SetTarget(target); 
        }
        else
        {
            projectile.Speed *= 2f;
        }
        
        var fireballDir = (target.position - FireballLocation.position).normalized;
        var targetRotation = Quaternion.LookRotation(fireballDir).eulerAngles;
        fireballGO.transform.localEulerAngles = targetRotation;
    }

    /// <summary>
    /// Shoot a fireball at a position instead. Does not allow for homing
    /// </summary>
    /// <param name="target"></param>
    public void SpawnFireball(Vector3 target)
    {
        var fireballGO = Instantiate(FireballPF, FireballLocation.position, Quaternion.identity);
        var projectile = fireballGO.GetComponent<Projectile>();
        projectile.Speed *= 2f;

        var fireballDir = (target - FireballLocation.position).normalized;
        var targetRotation = Quaternion.LookRotation(fireballDir).eulerAngles;
        fireballGO.transform.localEulerAngles = targetRotation;
    }
}
