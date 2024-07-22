using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{
    public class DoubleTailSwipe : AI_State
    {
        public float Damage = 30f;
        private float _windupDuration = 0.15f;
        private float _rotationDuration = 0.6f;
        private float _totalDurationOfEachSwipe;

        private string _animationState = "Double Tail Swipe";
        private TargetDirection _directionOfPlayer;
        private float _timeOfLastSwipe;

        private bool _isRotating;
        private float _rotationDegrees;
        private float _elapsedRotationTime;

        private Quaternion _startingRotation;
        private Quaternion _targetRotation;

        public DoubleTailSwipe(EnemyController myController) : base(myController)
        {
             var duration = _myController.GetStateDuration(_animationState);
            _totalDurationOfEachSwipe = (duration.HasValue) ? duration.Value / 2f : 1f;

            _directionOfPlayer = DirectionOfTarget.GetDirectionOfTarget(_myController.transform, _myController.TargetTransform);
            if (_directionOfPlayer == TargetDirection.Right || _directionOfPlayer == TargetDirection.Backward)
            {
                _rotationDegrees = 180;
                _myController.FlipEnemyScale();
            }
            else
            {
                _rotationDegrees = -180;
            }

            _isRotating = false;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _timeOfLastSwipe = _timeAtStateEnter;
            PlayAnimationState(_animationState);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            if (_isRotating)
            {
                _elapsedRotationTime += Time.deltaTime;
                float t = Mathf.Clamp01(_elapsedRotationTime / _rotationDuration);
                _myController.transform.rotation = Quaternion.Slerp(_startingRotation, _targetRotation, t);

                if (t >= 1.0f)
                {
                    DeactiveAttack();
                    _isRotating = false;
                }
            }
            else
            {
                if (Time.time - _timeOfLastSwipe > _windupDuration)
                {
                    _isRotating = true;
                    _elapsedRotationTime = 0f;
                    _startingRotation = _myController.transform.rotation;
                    _targetRotation = _myController.transform.rotation * Quaternion.Euler(0, _rotationDegrees, 0);
                    _timeOfLastSwipe = Time.time + _totalDurationOfEachSwipe;

                    ActiveAttack();
                }
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

        private void ActiveAttack()
        {
            ((DragonController)_myController).Tail.ActivateWeapon(Damage);
        }

        private void DeactiveAttack()
        {
            ((DragonController)_myController).Tail.DeactivateWeapon();
        }
    }
}
