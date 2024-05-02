using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdleState : AI_State
{
    public float MinIdleTime = 1.5f;
    public float MaxIdleTime = 3;

    public bool IsInIdle;
    private float _idleTime;
    private bool _hasAlreadyMoved;

    public IdleState(EnemyController myController) : base(myController)
    {
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);

        // shorten delay after a basic action
        if (fromAction == "BasicAttack")
        {
            MinIdleTime = 0.6f; MaxIdleTime = 1f;
        }
        else if (fromAction == "Hurt")
        {
            MinIdleTime = 0.4f; MaxIdleTime = 0.7f;
        }

        _idleTime = Random.Range(MinIdleTime, MaxIdleTime);
        _myController.UpdateMovementParameters(0f, 0f, false);

        IsInIdle = false;
        _hasAlreadyMoved = false;
        // All animations will naturally return to Idle anim state.
        // Wait for it to do so
    }

    public override void Update(float delta)
    {
        // Wait until animation is back in idle
        IsInIdle = _animator.GetBool("Idle");
        if (IsInIdle == false) return;

        if (_stateActive == false) return;

        if (Time.time - _timeSinceStateEnter >= _idleTime)
        {
            _myController.RestoreEnemyScale();

            if (!_hasAlreadyMoved && _lastAction != "BackstepFireball")
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
        
        // Once in range, check action history.
        // If basic attack was used less than twice, use it again.
        // Otherwise, check if the other options can be used.
        // ONLY use basic attack again if no others are applicable
        if (inRange)
        {
            var actionHistory = _myController.GetActionHistory();
            if (actionHistory.Where(e => e.Equals("BasicAttack")).Count() >= 3)
            {
                CheckBackstep();
                if (!_stateActive) return;

                CheckFireball();
                if (!_stateActive) return;
            }

            MoveState("BasicAttack");
        }
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
            if (Random.Range(0, 4) < 2)
            {
                MoveState("Fireball");
            }
        }
    }
}
