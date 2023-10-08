using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AI_State
{
    public float DelayBeforeAttackActive;
    public float Damage;
    public bool IsBite;

    private float _timeSinceEntering;
    private Quaternion _targetRotationToPlayer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        GetRotationToTarget();
        _timeSinceEntering = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        RotateToPlayer();
        _timeSinceEntering += Time.deltaTime;
        if (_timeSinceEntering >= DelayBeforeAttackActive)
        {
            ActiveAttack();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        DeactiveAttack();
    }

    private void GetRotationToTarget()
    {
        Vector3 targetDir = (_enemyController.TargetTransform.position - _transform.position).normalized;
        _targetRotationToPlayer = Quaternion.LookRotation(targetDir);
    }

    private bool RotateToPlayer()
    {
        var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotationToPlayer, _enemyController.TurnSpeed * Time.deltaTime).eulerAngles;
        rotateVector.x = 0f;

        if (rotateVector.magnitude > 0.5f)
        {
            _transform.localEulerAngles = rotateVector;

            return true;
        }

        return false;
    }

    private void ActiveAttack()
    {
        if (IsBite)
        {
            _enemyController.Bite.ActivateWeapon(Damage);
        }
        else
        {
            _enemyController.Tail.ActivateWeapon(Damage);
        }
    }

    private void DeactiveAttack()
    {
        _enemyController.Bite.DeactivateWeapon();
        _enemyController.Tail.DeactivateWeapon();
    }
}
