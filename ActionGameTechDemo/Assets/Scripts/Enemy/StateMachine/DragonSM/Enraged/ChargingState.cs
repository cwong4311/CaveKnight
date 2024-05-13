using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{
    public class ChargingState : AI_State
    {
        public float DelayBeforeChargeActive = 0.5f;
        public float Damage = 50f;

        private string _animationState = "ChargeAttack";
        private Quaternion _targetRotationToPlayer;

        private int _chargeCount = 0;
        private const int MAX_CHARGE = 3;
        private float _durationPerCharge = 1.6f; // in seconds
        private float _delayBetweenCharge = 0.6f; // in seconds 
        private bool _isCharging = false;
        private bool _isRevving = false;
        private float _timeOfLastChargeStart;
        private Vector3 _previousFacingDirection;

        public ChargingState(EnemyController myController) : base(myController)
        {
            _chargeCount = 0;
            _isCharging = false;
            _isRevving = false;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            WaitOnActionCompleted();

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta)
        {
            if (_chargeCount > MAX_CHARGE)
            {
                MoveState("Idle");
                return;
            }

            if (_isCharging)
            {
                ActiveAttack();

                if (Time.time - _timeOfLastChargeStart > _durationPerCharge)
                {
                    if (GetChargeContinueChance(_chargeCount))
                    {
                        _chargeCount++;
                        _isCharging = false;
                        _isRevving = true;

                        _previousFacingDirection = _myController.transform.forward;
                    }
                    else
                    {
                        _chargeCount = MAX_CHARGE + 1;
                    }
                }

                RotateToPlayer(0.1f);
                ChargeInDirection(_myController.transform.forward, 2f);
                return;
            }

            if (_isRevving)
            {
                _myController.RB.velocity = Vector3.zero;

                DeactiveAttack();

                if (Time.time - _timeOfLastChargeStart > _durationPerCharge + _delayBetweenCharge)
                {
                    _isCharging = true;
                    _isRevving = false;
                    _timeOfLastChargeStart = Time.time;
                }

                RotateToPlayer(1.2f);
                ChargeInDirection(_previousFacingDirection, 1.1f);
                return;
            }

            if (Time.time - _timeAtStateEnter > DelayBeforeChargeActive)
            {
                _chargeCount++;
                _isCharging = true;
                _isRevving = false;
                _timeOfLastChargeStart = Time.time;
            }
            else
            {
                RotateToPlayer(1.4f);
            }
        }

        public override void OnStateExit(string toAction)
        {
            SetActionCompleted();
            DeactiveAttack();
        }

        private bool RotateToPlayer(float rotationSpeedMultiplier)
        {
            Vector3 targetDir = (_myController.TargetTransform.position - _transform.position).normalized;
            _targetRotationToPlayer = Quaternion.LookRotation(targetDir);

            var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotationToPlayer, _myController.TurnSpeed * Time.deltaTime * rotationSpeedMultiplier).eulerAngles;
            rotateVector.x = 0f;

            if (rotateVector.magnitude > 0.5f)
            {
                _transform.localEulerAngles = rotateVector;

                return true;
            }

            return false;
        }

        private void ChargeInDirection(Vector3 dodgeVelocity, float speedModifier)
        {
            _myController.RB.velocity = dodgeVelocity * _myController.ChaseSpeed * speedModifier;
        }

        private bool GetChargeContinueChance(int chargedCount)
        {
            return UnityEngine.Random.Range(0, chargedCount + 2) < 2;
        }

        private void ActiveAttack()
        {
            _myController.Chest.ActivateWeapon(Damage);
        }

        private void DeactiveAttack()
        {
            _myController.Chest.DeactivateWeapon();
        }

    }
}
