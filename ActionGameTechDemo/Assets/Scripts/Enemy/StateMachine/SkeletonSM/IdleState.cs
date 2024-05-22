using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Skeleton
{
    public class IdleState : AI_State
    {
        public float MinIdleTime = 0.8f;
        public float MaxIdleTime = 1.4f;

        public bool IsInIdle;
        private float _idleTime;

        private float _strafeTimeRemaining = 0f;
        private float _strafeDirection; // 1 is right, -1 is left

        // Follow primitive non-random Logic, in this order:
        // 1 - Go Close to Player (if already in melee then skip)
        // 2 - Strafe around player (strafe time is random)
        // 3 - Attack player; go back to 1
        private int _actionStep = 1;

        public IdleState(EnemyController myController) : base(myController)
        {
            _stateType = AIStateType.Idle;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _idleTime = UnityEngine.Random.Range(MinIdleTime, MaxIdleTime);
            _myController.UpdateMovementParameters(0f, 0f, false);

            IsInIdle = false;
            _actionStep = 1;
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

                PerformAction();
            }
        }

        public override void OnStateExit(string toAction)
        {
            ((SkeletonController)_myController).NavMeshAgent.ResetPath();
        }

        private void PerformAction()
        {
            ((SkeletonController)_myController).NavMeshAgent.updateRotation = true;

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

                return;
            }

            if (_actionStep == 1)
            {
                var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
                if (distance > _myController.MinDistance && distance < _myController.MaxDistance)
                {
                    MoveToTarget();
                }
                else if (distance > _myController.MaxDistance)
                {
                    _myController.TargetTransform = null;
                }
                else
                {
                    _actionStep = 2;
                    _strafeTimeRemaining = UnityEngine.Random.Range(1, 3);
                    _strafeDirection = GetStrafeDirection();
                }
            }
            else if (_actionStep == 2)
            {
                //((SkeletonController)_myController).NavMeshAgent.updateRotation = false;
                //_myController.UpdateMovementParameters(0.5f, 0f, false);

                ////RotateToPlayer();
                //((SkeletonController)_myController).NavMeshAgent.Move(
                //    _myController.transform.right * _strafeDirection * _myController.ChaseSpeed * Time.deltaTime);

                //_strafeTimeRemaining -= Time.deltaTime;
                //if (_strafeTimeRemaining <= 0)
                //{
                //    _myController.UpdateMovementParameters(0f, 0f, false);
                //    _actionStep = 3;
                //}

                _actionStep = 3;
            }
            else
            {
                MoveState("BasicAttack");
            }
        }

        private void MoveToTarget()
        {
            _myController.UpdateMovementParameters(1f, 0f);
            ((SkeletonController)_myController).MoveToDestination(_myController.TargetTransform.position);
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

        private float GetStrafeDirection()
        {
            // Raycast right 10f. If it hits, not enough position. Fly left
            Ray rightRay = new Ray(_transform.position, _transform.right);
            if (Physics.Raycast(rightRay, out var hit, 20f, LayerMask.GetMask("Environment")))
            {
                return -1;
            }
            // If it doesn't hit, enough space. Fly right
            else
            {
                return 1;
            }
        }
    }
}
