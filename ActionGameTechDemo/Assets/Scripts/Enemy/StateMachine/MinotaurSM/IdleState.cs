using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Minotaur
{
    public class IdleState : AI_State
    {
        public float MinIdleTime = 0.6f;
        public float MaxIdleTime = 0.9f;

        public bool IsInIdle;
        private float _idleTime;

        private bool _hasMoved;
        private bool _isStrafing;
        private float _strafeTimeRemaining = 0f;
        private float _strafeDirection; // 1 is right, -1 is left

        private NavMeshAgent _myNavAgent;
        private float _originalNavAgentSpeed = 0f;

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

            if (fromAction == "Hurt")
            {
                MinIdleTime = 0.1f; MaxIdleTime = 0.2f;
            }
            if (fromAction == "Kick")
            {
                MinIdleTime = 0.8f; MaxIdleTime = 1.2f;
            }

            _idleTime = UnityEngine.Random.Range(MinIdleTime, MaxIdleTime);
            _myController.UpdateMovementParameters(0f, 0f, false);

            _myNavAgent = ((MinotaurController)_myController).NavMeshAgent;
            _originalNavAgentSpeed = _myNavAgent.speed;

            IsInIdle = false;
            _hasMoved = false;
            _isStrafing = false;
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

                PerformAction(delta);
            }
        }

        public override void OnStateExit(string toAction)
        {
            if (_myNavAgent.hasPath)
            {
                _myNavAgent.ResetPath();
            }
        }

        private void PerformAction(float delta)
        {
            _myNavAgent.updateRotation = true;

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

            // Get the distance between this enemy and the target (player)
            var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
            if (_actionStep == 1)
            {
                // If there's a target but it's too far, move to it
                if (distance > (1.3 * _myController.MinDistance) && distance < _myController.MaxDistance)
                {
                    _hasMoved = true;
                    MoveToTarget();
                }
                // If it moves out of range stop pursuing
                else if (distance > _myController.MaxDistance)
                {
                    _myController.TargetTransform = null;
                }
                // Once we are at melee range:
                // 1. If we started Idle state in melee range, strafe
                else if (_hasMoved == false)
                {
                    if (!_isStrafing)
                    {
                        _strafeTimeRemaining = UnityEngine.Random.Range(0.5f, 1.5f);
                        _strafeDirection = GetStrafeDirection();
                        if (_myNavAgent.hasPath) _myNavAgent.ResetPath();

                        _isStrafing = true;
                    }

                    _myNavAgent.updateRotation = false;
                    _myController.UpdateMovementParameters(0.5f, 0f, false);

                    RotateToPlayer();
                    _myNavAgent.Move(_myController.transform.right * _strafeDirection * _myNavAgent.speed * 0.2f * delta);

                    // Strafe completed, move to 3rd attack step
                    _strafeTimeRemaining -= delta;
                    if (_strafeTimeRemaining <= 0)
                    {
                        _actionStep = 3;
                    }
                }
                // 2. If we had to run up to target; move to 2nd attack step
                else
                {
                    if (_myNavAgent.hasPath) _myNavAgent.ResetPath();
                    _actionStep = 2;
                }
            }
            // If enemy had to close gap, try to do dash attack
            else if (_actionStep == 2)
            {
                // If dash attack is not valid, move to basic attacks
                if (CheckKick() == false)
                {
                    _actionStep = 3;
                }
            }
            // Perform basic attack
            else
            {
                if (distance > _myController.MinDistance)
                {
                    _myController.UpdateMovementParameters(1f, 0f, false);
                    MoveToTarget();
                    return;
                }

                _myController.UpdateMovementParameters(0f, 0f, false);

                bool hasAttacked = false;

                // 50-50 perform Attack 1 or Attack 2
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    // If attack 1 isn't valid, do Attack 2
                    if (CheckAttack1() == false)
                    {
                        hasAttacked = CheckAttack2();
                    }
                }
                else
                {
                    // If attack 2 isn't valid, do Attack 1
                    if (CheckAttack2() == false)
                    {
                        hasAttacked = CheckAttack1();
                    }
                }

                // If neither attack is valid, just do Fury Attack
                if (hasAttacked == false)
                {
                    MoveState("Kick");
                }
            }
        }

        private void MoveToTarget()
        {
            _myController.UpdateMovementParameters(1f, 0f);
            ((MinotaurController)_myController).MoveToDestination(_myController.TargetTransform.position);
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
            var directionMod = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;

            Ray rightRay = new Ray(_transform.position, _transform.right * directionMod);
            if (Physics.Raycast(rightRay, out var hit, 20f, LayerMask.GetMask("Environment")))
            {
                return -1 * directionMod;
            }
            // If it doesn't hit, enough space. Fly right
            else
            {
                return 1 * directionMod;
            }
        }

        // Attacks
        private bool CheckKick()
        {
            if (GetTimesRecentlyExecuted("Kick") < 2)
            {
                MoveState("Kick");
                return true;
            }
            return false;
        }

        private bool CheckAttack1()
        {
            if (GetTimesRecentlyExecuted("Attack1") < 2)
            {
                MoveState("Attack1");
                return true;
            }
            return false;
        }

        private bool CheckAttack2()
        {
            if (GetTimesRecentlyExecuted("Attack2") < 2)
            {
                MoveState("Attack2");
                return true;
            }
            return false;
        }
    }
}
