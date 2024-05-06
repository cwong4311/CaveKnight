using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball: MonoBehaviour
{
    // TO DO: Fix this
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
}
