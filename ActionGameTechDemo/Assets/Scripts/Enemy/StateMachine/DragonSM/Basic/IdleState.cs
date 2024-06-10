using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Dragon
{
    public class IdleState : AI_State
    {
        public float MinIdleTime = 1.5f;
        public float MaxIdleTime = 3;

        public bool IsInIdle;
        private float _idleTime;
        private bool _hasAlreadyMoved;

        public IdleState(EnemyController myController) : base(myController)
        {
            _stateType = AIStateType.Idle;
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

            _idleTime = UnityEngine.Random.Range(MinIdleTime, MaxIdleTime);
            _myController.UpdateMovementParameters(0f, 0f, false);

            IsInIdle = false;
            _hasAlreadyMoved = false;
            // All animations will naturally return to Idle anim state.
            // Wait for it to do so
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            // Wait until animation is back in idle
            IsInIdle = _animator.GetBool("Idle");
            if (IsInIdle == false) return;

            if (_stateActive == false) return;

            // Always enable gravity when back in Idle state
            _myController.ToggleGravity(true);

            if (Time.time - _timeAtStateEnter >= _idleTime)
            {
                _myController.RestoreEnemyScale();

                if (!_hasAlreadyMoved && _myController.LastPerformedAction != "Scream")
                {
                    CheckScream();
                }

                if (!_hasAlreadyMoved && _myController.LastPerformedAction != "BackstepFireball")
                {
                    CheckBackstep();
                }

                if (!_hasAlreadyMoved && _myController.LastPerformedAction != "Fireball")
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
                // If no current target, AND no players nearby, don't do anyting else
                if (_myController.IsAnyPlayerNearby() == false) return;

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
                if (GetTimesRecentlyExecuted("BasicAttack") >= 3)
                {
                    var alternativeChecklist = new List<Action>() {
                        () => CheckScream(),
                        () => CheckBackstep(),
                        () => CheckFireball(),
                    };

                    foreach (var altAction in alternativeChecklist)
                    {
                        altAction.Invoke();
                        if (!_stateActive) return;
                    }
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

            _transform.localEulerAngles = Vector3.up * rotateVector.y;
            _myController.RB.velocity = _transform.forward * _myController.ChaseSpeed;
        }

        private void CheckScream()
        {
            if (_myController.TargetTransform == null) return;

            var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
            if (distance <= _myController.MinDistance)
            {
                // 5% to trigger fake scream
                if (UnityEngine.Random.Range(0, 10) == 0)
                {
                    MoveState("GroundedScream");
                }
            }
        }

        private void CheckBackstep()
        {
            if (_myController.TargetTransform == null) return;

            var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
            if (distance <= _myController.MinDistance)
            {
                if (UnityEngine.Random.Range(0, 4) < 2)
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
                if (UnityEngine.Random.Range(0, 4) < 2)
                {
                    MoveState("Fireball");
                }
            }
        }
    }
}
