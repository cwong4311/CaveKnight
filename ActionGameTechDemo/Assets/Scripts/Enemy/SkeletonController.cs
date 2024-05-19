using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonController : CharacterManager
{
    public Transform TargetTransform;

    private NavMeshAgent agent;

    public void Awake()
    {
        TargetTransform = GameObject.FindObjectOfType<PlayerController>().transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public override void TriggerHitStop(float damageAmount, bool isAttacker)
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        agent?.SetDestination(TargetTransform.position);
    }
}
