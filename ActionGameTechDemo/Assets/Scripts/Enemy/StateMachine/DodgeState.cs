using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : AI_State
{
    private float _delayBeforeMoving = 0.6f;

    private float _dodgeTime = 0f;

    private string _backstepAnimationState = "Backstep";

    private Vector3? _dodgeVelocity;

    public DodgeState(EnemyController myController) : base(myController)
    {
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);
        _dodgeTime = 0f;
        _dodgeVelocity = null;

        // Go to dodge anim state
        PlayAnimationState(_backstepAnimationState);
    }

    public override void Update(float delta)
    {
        _dodgeTime += delta;
        if (_dodgeTime > _delayBeforeMoving)
        {
            if (_dodgeVelocity.HasValue == false)
            {
                _dodgeVelocity = GetDodgeDirection();
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
            }

            DodgeInDirection(_dodgeVelocity.Value);
            RotateToPlayer();
        }

        if (IsAnimationCompleted(_backstepAnimationState))
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);
            MoveState("BackstepFireball");
        }
    }

    public override void OnStateExit(string toAction) { }

    private void DodgeInDirection(Vector3 dodgeVelocity)
    {
        _myController.RB.velocity = dodgeVelocity;
    }

    private Vector3 GetDodgeDirection()
    {
        Vector3 dodgeVelocity = Vector3.zero;

        // 50-50 chance to dodge front/back or left/right
        var rand = Random.Range(0, 2);
        if (rand == 0)
        {
            // Raycast forward 10f. If it hits, not enough position. Fly backwards
            Ray forwardRay = new Ray(_transform.position, _transform.forward);
            if (Physics.Raycast(forwardRay, out var hit, 20f, LayerMask.GetMask("Environment")))
            {
                dodgeVelocity = -1 * _transform.forward * _myController.ChaseSpeed * 2.5f;
            }
            // If it doesn't hit, enough space. Fly forwards
            else
            {
                dodgeVelocity = _transform.forward * _myController.ChaseSpeed * 2.5f;
            }
        }
        else
        {
            // Raycast right 10f. If it hits, not enough position. Fly left
            Ray rightRay = new Ray(_transform.position, _transform.right);
            if (Physics.Raycast(rightRay, out var hit, 20f, LayerMask.GetMask("Environment")))
            {
                dodgeVelocity = -1 * _transform.right * _myController.ChaseSpeed * 2.5f;
            }
            // If it doesn't hit, enough space. Fly right
            else
            {
                dodgeVelocity = _transform.right * _myController.ChaseSpeed * 2.5f;
            }
        }

        return dodgeVelocity;
    }

    private bool RotateToPlayer()
    {
        Vector3 targetDir = (_myController.TargetTransform.position - _transform.position).normalized;
        var _targetRotationToPlayer = Quaternion.LookRotation(targetDir);
        var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotationToPlayer, _myController.TurnSpeed * Time.deltaTime).eulerAngles;
        rotateVector.x = 0f;

        if (rotateVector.magnitude > 0.5f)
        {
            _transform.localEulerAngles = rotateVector;

            return true;
        }

        return false;
    }
}
