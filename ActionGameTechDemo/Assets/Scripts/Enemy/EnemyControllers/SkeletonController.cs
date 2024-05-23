using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonController : EnemyController
{
    [Header("Enemy-Specific Hitboxes")]
    public WeaponDamager Sword;

    [Header("NavMesh")]
    public NavMeshAgent NavMeshAgent;

    public override void Awake()
    {
        base.Awake();

        NavMeshAgent.speed = ChaseSpeed;
        Sword.SetWeaponTarget(targetEnemy: false);
    }

    public void MoveToDestination(Vector3 position)
    {
        NavMeshAgent?.SetDestination(position);
    }
}
