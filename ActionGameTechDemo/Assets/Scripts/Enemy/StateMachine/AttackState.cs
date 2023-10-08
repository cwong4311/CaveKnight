using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AI_State
{
    private Quaternion _targetRotationToPlayer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        GetRotationToTarget();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        RotateToPlayer();
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
}
