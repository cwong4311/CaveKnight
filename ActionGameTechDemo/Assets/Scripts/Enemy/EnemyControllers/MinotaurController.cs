using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MinotaurController: EnemyController
{
    [Header("Enemy-Specific Hitboxes")]
    public WeaponDamager Axe;
    public WeaponDamager Foot;

    [Header("NavMesh")]
    public NavMeshAgent NavMeshAgent;

    public override void Awake()
    {
        base.Awake();

        NavMeshAgent.speed = ChaseSpeed;
        Axe.SetWeaponTarget(targetEnemy: false);
        Foot.SetWeaponTarget(targetEnemy: false);
    }

    public void MoveToDestination(Vector3 position)
    {
        NavMeshAgent?.SetDestination(position);
    }
}
