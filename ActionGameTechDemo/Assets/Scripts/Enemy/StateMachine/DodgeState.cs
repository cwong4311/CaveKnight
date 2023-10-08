using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : AI_State
{
    [SerializeField]
    private float _delayBeforeMoving;

    private float _dodgeTime = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _dodgeTime = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _dodgeTime += Time.deltaTime;
        if (_dodgeTime > _delayBeforeMoving)
        {
            DodgeBackwards();
        }
    }

    private void DodgeBackwards()
    {
        _enemyController.RB.velocity = -1 * _transform.forward * _enemyController.ChaseSpeed * 1.5f;
    }
}
