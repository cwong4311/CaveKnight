using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Minotaur
{
    public class SecondaryAttackState : AI_State
    {
        private int _delayInFrames = 42;
        private int _untilInFrames = 60;
        public float DelayBeforeAttackActive => FramesToTime(_delayInFrames);
        public float AttackActiveUntil => FramesToTime(_untilInFrames);

        public float Damage = 60f;

        private string _animationState = "Attack_2";
        public bool _hasAttacked = false;

        private Quaternion _targetRotationToPlayer;

        public SecondaryAttackState(EnemyController myController) : base(myController)
        {
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            GetRotationToTarget();
            PlayAnimationState(_animationState, 0.05f);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            if (Time.time - _timeAtStateEnter >= AttackActiveUntil)
            {
                DeactiveAttack();
            }
            else if (Time.time - _timeAtStateEnter >= DelayBeforeAttackActive)
            {
                ActiveAttack();
            }
            else
            {
                RotateToPlayer();
            }

            if (IsAnimationCompleted(_animationState))
            {
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction)
        {
            DeactiveAttack();
            _hasAttacked = false;
        }

        private void GetRotationToTarget()
        {
            Vector3 targetDir = (_myController.TargetTransform.position - _transform.position).normalized;
            _targetRotationToPlayer = Quaternion.LookRotation(targetDir);
        }

        private bool RotateToPlayer()
        {
            var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotationToPlayer, _myController.TurnSpeed * Time.deltaTime).eulerAngles;

            if (rotateVector.magnitude > 0.5f)
            {
                _transform.localEulerAngles = Vector3.up * rotateVector.y;

                return true;
            }

            return false;
        }

        private void ActiveAttack()
        {
            if (_hasAttacked == false)
            {
                ((MinotaurController)_myController).Axe.ActivateWeapon(Damage);
                _hasAttacked = true;
            }
        }

        private void DeactiveAttack()
        {
            if (_hasAttacked)
            {
                ((MinotaurController)_myController).Axe.DeactivateWeapon();
                _hasAttacked = false;
            }
        }
    }
}