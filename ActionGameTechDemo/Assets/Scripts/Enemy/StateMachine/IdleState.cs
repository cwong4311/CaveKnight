using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AI_State
{
    public float MinIdleTime;
    public float MaxIdleTime;

    private float _idleTime;
    private bool _hasAlreadyMoved;

    private string _lastPerformedAction = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _idleTime = Random.Range(MinIdleTime, MaxIdleTime);

        _enemyController.RegisterState("Idle");
        _enemyController.UpdateMovementParameters(0f, 0f, false);

        _hasAlreadyMoved = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_stateActive == false) return;

        if (Time.time - _timeSinceStateEnter >= _idleTime)
        {
            if (!_hasAlreadyMoved && _enemyController.LastPerformedAction != "Backstep")
            {
                CheckBackstep();
            }

            if (!_hasAlreadyMoved)
            {
                CheckFireball();
            }


            PerformAction();
        }
    }

    private void PerformAction()
    {
        bool inRange = false;

        if (_enemyController.TargetTransform == null)
        {
            var colliders = Physics.OverlapSphere(_animator.transform.position, _enemyController.PlayerDetectionRange);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    _enemyController.TargetTransform = collider.transform;
                }
            }

            if (_enemyController.TargetTransform == null)
            {
                RandomRoam();
                _hasAlreadyMoved = true;
            }
        }
        else
        {
            var distance = Vector3.Distance(_transform.position, _enemyController.TargetTransform.position);
            if (distance > _enemyController.MinDistance && distance < _enemyController.MaxDistance)
            {
                MoveToTarget();
                _hasAlreadyMoved = true;
            }
            else if (distance > _enemyController.MaxDistance)
            {
                _enemyController.TargetTransform = null;
            }
            else
            {
                _enemyController.UpdateMovementParameters(0f, 0f, false);
                inRange = true;
            }
        }
        
        if (inRange && UsableCommands.Length > 0)
        {
            var rand = Random.Range(0, UsableCommands.Length);
            var nextAction = UsableCommands[rand];
            MoveState(nextAction);
        }
    }

    private void RandomRoam()
    {
        _enemyController.UpdateMovementParameters(0.5f, 0f);
        // TO DO
    }

    private void MoveToTarget()
    {
        _enemyController.UpdateMovementParameters(1f, 0f);
        Vector3 targetDir = (_enemyController.TargetTransform.position - _transform.position).normalized;

        var targetRotation = Quaternion.LookRotation(targetDir);
        var rotateVector = Quaternion.Slerp(_transform.rotation, targetRotation, _enemyController.TurnSpeed * Time.deltaTime).eulerAngles;
        rotateVector.x = 0f;

        _transform.localEulerAngles = rotateVector;
        _enemyController.RB.velocity = _transform.forward * _enemyController.ChaseSpeed;
    }

    private void CheckBackstep()
    {
        if (_enemyController.TargetTransform == null) return;

        var distance = Vector3.Distance(_transform.position, _enemyController.TargetTransform.position);
        if (distance <= _enemyController.MinDistance)
        {
            if (Random.Range(0, 3) > 0)
            {
                MoveState("Backstep");
            }
        }
    }

    private void CheckFireball()
    {
        if (_enemyController.TargetTransform == null) return;

        var distance = Vector3.Distance(_transform.position, _enemyController.TargetTransform.position);
        if (distance > _enemyController.MinDistance * 1.5f)
        {
            if (Random.Range(0, 3) == 0)
            {
                MoveState("Fireball");
            }
        }
    }
}
