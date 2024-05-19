using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonController : CharacterManager
{
    public Transform TargetTransform;

    public NavMeshAgent NavMeshAgent;

    public void Awake()
    {
        TargetTransform = GameObject.FindObjectOfType<PlayerController>().transform;
    }

    public override void TriggerHitStop(float damageAmount, bool isAttacker)
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        NavMeshAgent?.SetDestination(TargetTransform.position);
    }
}
