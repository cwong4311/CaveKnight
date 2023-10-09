using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireball: MonoBehaviour
{
    // TO DO: Fix this
    public Transform FireballLocation;
    public GameObject FireballPF;

    public void SpawnFireball(Transform target)
    {
        var fireballGO = Instantiate(FireballPF, FireballLocation.position, Quaternion.identity);
        fireballGO.GetComponent<Projectile>().SetTarget(target);
        var fireballDir = (target.position - FireballLocation.forward).normalized;

        fireballGO.transform.forward = FireballLocation.forward;
    }
}
