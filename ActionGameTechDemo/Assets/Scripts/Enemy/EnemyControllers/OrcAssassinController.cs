using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class OrcAssassinController: EnemyController
{
    [Header("Enemy-Specific Hitboxes")]
    public WeaponDamager MainDagger;
    public WeaponDamager OffhandDagger;

    [Header("NavMesh")]
    public NavMeshAgent NavMeshAgent;

    public override void Awake()
    {
        base.Awake();

        NavMeshAgent.speed = ChaseSpeed;
        MainDagger.SetWeaponTarget(targetEnemy: false);
        OffhandDagger.SetWeaponTarget(targetEnemy: false);
    }

    public void MoveToDestination(Vector3 position)
    {
        NavMeshAgent?.SetDestination(position);
    }
}
