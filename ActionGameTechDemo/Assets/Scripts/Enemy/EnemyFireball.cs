using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireball: MonoBehaviour
{
    public Transform FireballLocation;
    public GameObject FireballPF;

    public void SpawnFireball(Vector3 target)
    {
        var fireballGO = Instantiate(FireballPF, FireballLocation.position, Quaternion.identity);
        var fireballDir = (target - FireballLocation.forward).normalized;

        fireballGO.transform.forward = FireballLocation.forward;
    }
}
