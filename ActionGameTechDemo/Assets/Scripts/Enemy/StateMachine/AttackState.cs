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

    public AttackState(EnemyController myController) : base(myController)
    {
    }

    public override void OnStateEnter(string fromAction)
    {
        GetRotationToTarget();
        _timeSinceEntering = 0f;

        // Go to correct animation state
    }

    public override void Update(float delta)
    {
        RotateToPlayer();
        _timeSinceEntering += delta;
        if (_timeSinceEntering >= DelayBeforeAttackActive)
        {
            ActiveAttack();
        }
    }

    public override void OnStateExit(string toAction)
    {
        DeactiveAttack();
    }

    private void GetRotationToTarget()
    {
        Vector3 targetDir = (_myController.TargetTransform.position - _transform.position).normalized;
        _targetRotationToPlayer = Quaternion.LookRotation(targetDir);
    }

    private bool RotateToPlayer()
    {
        var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotationToPlayer, _myController.TurnSpeed * Time.deltaTime).eulerAngles;
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
            _myController.Bite.ActivateWeapon(Damage);
        }
        else
        {
            _myController.Tail.ActivateWeapon(Damage);
        }
    }

    private void DeactiveAttack()
    {
        _myController.Bite.DeactivateWeapon();
        _myController.Tail.DeactivateWeapon();
    }
}
