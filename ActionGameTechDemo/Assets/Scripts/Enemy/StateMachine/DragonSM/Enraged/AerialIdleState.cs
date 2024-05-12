using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AI.Dragon
{
    public class AerialIdleState : AI_State
    {
        public float MinIdleTime = 0.3f;
        public float MaxIdleTime = 0.6f;

        public bool IsInIdle;
        private float _idleTime;

        private bool _actionDecided = false;
        private Vector3 _lastEnemyPosition;

        private bool _isRandomMove;
        private float _randomMoveDuration;
        private Vector3 _randomMoveDirection;
        private Vector3 _randomMovePosition;
        private Action _randomMoveEndAction;
        private float _randomMoveStartTime;

        public AerialIdleState(EnemyController myController) : base(myController)
        {
            _stateType = AIStateType.Idle;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _idleTime = UnityEngine.Random.Range(MinIdleTime, MaxIdleTime);
            _myController.UpdateMovementParameters(0f, 0f, false);

            IsInIdle = false;
            _actionDecided = false;
            _isRandomMove = false;
            // All animations will naturally return to Idle anim state.
            // Wait for it to do so
        }

        public override void Update(float delta)
        {
            // Wait until animation is back in idle
            IsInIdle = _animator.GetBool("Idle");
            if (IsInIdle == false) return;

            if (_stateActive == false) return;

            if (_isRandomMove)
            {
                ProcessRandomMove();
            }

            if (_actionDecided == true) return;

            if (Time.time - _timeAtStateEnter >= _idleTime)
            {
                _myController.RestoreEnemyScale();
                
                var distance = CheckEnemyDistance();
                if (distance.HasValue)
                {
                    _lastEnemyPosition = _myController.TargetTransform.position;
                    PerformAction(distance.Value);
                }
            }
        }

        public override void OnStateExit(string toAction) { }

        private float? CheckEnemyDistance()
        {
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
                if (distance > _myController.MaxDistance)
                {
                    _myController.TargetTransform = null;
                }
                else
                {
                    return distance;
                }
            }

            return null;
        }

        private void PerformAction(float distance)
        {
            // fireballs #1 and #2 are guaranteed. #3 has 50% chance
            var fireballCount = GetTimesRecentlyExecuted("AerialFireball");
            var fireballRand = (fireballCount < 2) ? 0 : UnityEngine.Random.Range(0, 3);
            if (fireballCount < 3 && fireballRand == 0)
            {
                // If fireballs left to shoot, move first then shoot
                CheckMove(CheckFireball);
                Debug.Log($"TEST ---- Has fireballs. Moving to {_randomMovePosition}");
            }
            else
            {
                // If no fireball, land
                CheckLand();
                Debug.Log($"TEST ---- No fireballs. Landing");
            }

            _actionDecided = true;
        }

        private void CheckMove(Action moveEndAction)
        {
            _isRandomMove = true;
            _randomMoveDirection = GetMovementDirection();
            _randomMoveDuration = UnityEngine.Random.Range(0.6f, 1.2f);
            _randomMovePosition = _myController.transform.position + (_randomMoveDirection * _randomMoveDuration);
            _randomMoveEndAction = moveEndAction;
            _randomMoveStartTime = Time.time;

            ProcessRandomMove();
        }

        private void ProcessRandomMove()
        {
            if (Time.time - _randomMoveStartTime > _randomMoveDuration)
            {
                Debug.Log($"TEST ---- Movement End. Perform Action");
                _myController.UpdateMovementParameters(0f, 0f);
                _myController.RB.velocity = Vector3.zero;
                _randomMoveEndAction();
            }

            _myController.UpdateMovementParameters(1f, 0f);
            RotateToTarget(_randomMovePosition);
            _myController.RB.velocity = _randomMoveDirection;
        }

        private void CheckFireball()
        {
            if (_lastEnemyPosition == null)
            {
                _actionDecided = false;
                return;
            }

            Debug.Log($"TEST ---- Fireball");
            MoveState("AerialFireball");
        }

        private void CheckLand()
        {
            if (_lastEnemyPosition == null)
            {
                _actionDecided = false;
                return;
            }

            if (UnityEngine.Random.Range(0, 3) == 0)
            {
                Debug.Log($"TEST ---- Land");
                MoveState("Landing");
            }
            else
            {
                Debug.Log($"TEST ---- Divebomb");
                MoveState("DiveBomb");
            }
        }

        private Vector3 GetMovementDirection()
        {
            Vector3 dodgeVelocity = Vector3.zero;

            // 50-50 chance to dodge front/back or left/right
            var rand = UnityEngine.Random.Range(0, 2);
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

        private bool RotateToTarget(Vector3 targetPosition)
        {
            Vector3 targetDir = (targetPosition - _transform.position).normalized;
            var _targetRotation = Quaternion.LookRotation(targetDir);
            var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotation, _myController.TurnSpeed * Time.deltaTime).eulerAngles;
            rotateVector.x = 0f;

            if (rotateVector.magnitude > 0.5f)
            {
                _transform.localEulerAngles = rotateVector;

                return true;
            }

            return false;
        }
    }
}
