using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AI_State
{
    public float DelayBeforeAttackActive = 0.3f;
    public float Damage = 20f;

    private bool isPrimaryAttack;
    private string _primaryAttack = "Basic Attack";
    private string _secondaryAttack = "Tail Attack";

    private string _animationState;
    private Quaternion _targetRotationToPlayer;

    public AttackState(EnemyController myController, bool isPrimaryAttack) : base(myController)
    {
        this.isPrimaryAttack = isPrimaryAttack;
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);

        _animationState = (isPrimaryAttack) ? _primaryAttack : _secondaryAttack;
        PlayAnimationState(_animationState);

        GetRotationToTarget();
    }

    public override void Update(float delta)
    {
        RotateToPlayer();
        if (Time.time - _timeSinceStateEnter >= DelayBeforeAttackActive)
        {
            ActiveAttack();
        }

        if (IsAnimationCompleted(_animationState))
        {
            MoveState("Idle");
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
        if (isPrimaryAttack)
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
