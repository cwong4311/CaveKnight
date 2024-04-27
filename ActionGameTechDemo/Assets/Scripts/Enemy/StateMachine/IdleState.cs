using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AI_State
{
    public float MinIdleTime;
    public float MaxIdleTime;

    private float _idleTime;
    private bool _hasAlreadyMoved;

    public IdleState(EnemyController myController) : base(myController)
    {
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);

        _idleTime = Random.Range(MinIdleTime, MaxIdleTime);

        _myController.RegisterState("Idle");
        _myController.UpdateMovementParameters(0f, 0f, false);

        _hasAlreadyMoved = false;
        // TODO: Go To Idle Anim State
    }

    public override void Update(float delta)
    {
        if (_stateActive == false) return;

        if (Time.time - _timeSinceStateEnter >= _idleTime)
        {
            if (!_hasAlreadyMoved && _lastAction != "Backstep")
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

    public override void OnStateExit(string toAction) { }

    private void PerformAction()
    {
        bool inRange = false;

        if (_myController.TargetTransform == null)
        {
            var colliders = Physics.OverlapSphere(_animator.transform.position, _myController.PlayerDetectionRange);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    _myController.TargetTransform = collider.transform;
                }
            }

            if (_myController.TargetTransform == null)
            {
                RandomRoam();
                _hasAlreadyMoved = true;
            }
        }
        else
        {
            var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
            if (distance > _myController.MinDistance && distance < _myController.MaxDistance)
            {
                MoveToTarget();
                _hasAlreadyMoved = true;
            }
            else if (distance > _myController.MaxDistance)
            {
                _myController.TargetTransform = null;
            }
            else
            {
                _myController.UpdateMovementParameters(0f, 0f, false);
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
        _myController.UpdateMovementParameters(0.5f, 0f);
        // TO DO
    }

    private void MoveToTarget()
    {
        _myController.UpdateMovementParameters(1f, 0f);
        Vector3 targetDir = (_myController.TargetTransform.position - _transform.position).normalized;

        var targetRotation = Quaternion.LookRotation(targetDir);
        var rotateVector = Quaternion.Slerp(_transform.rotation, targetRotation, _myController.TurnSpeed * Time.deltaTime).eulerAngles;
        rotateVector.x = 0f;

        _transform.localEulerAngles = rotateVector;
        _myController.RB.velocity = _transform.forward * _myController.ChaseSpeed;
    }

    private void CheckBackstep()
    {
        if (_myController.TargetTransform == null) return;

        var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
        if (distance <= _myController.MinDistance)
        {
            if (Random.Range(0, 3) > 0)
            {
                MoveState("Backstep");
            }
        }
    }

    private void CheckFireball()
    {
        if (_myController.TargetTransform == null) return;

        var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
        if (distance > _myController.MinDistance * 1.5f)
        {
            if (Random.Range(0, 3) == 0)
            {
                MoveState("Fireball");
            }
        }
    }
}
