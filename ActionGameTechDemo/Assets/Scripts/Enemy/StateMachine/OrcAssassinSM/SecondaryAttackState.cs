using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.OrcAssassin
{
    public class SecondaryAttackState : AI_State
    {
        private int _delayInFrames = 25;
        private int _untilInFrames = 37;
        public float DelayBeforeAttackActive => FramesToTime(_delayInFrames);
        public float AttackActiveUntil => FramesToTime(_untilInFrames);

        public float Damage = 20f;

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
            rotateVector.x = 0f;

            if (rotateVector.magnitude > 0.5f)
            {
                _transform.localEulerAngles = rotateVector;

                return true;
            }

            return false;
        }

        private void ActiveAttack()
        {
            if (_hasAttacked == false)
            {
                ((OrcAssassinController)_myController).MainDagger.ActivateWeapon(Damage);
                _hasAttacked = true;
            }
        }

        private void DeactiveAttack()
        {
            if (_hasAttacked)
            {
                ((OrcAssassinController)_myController).MainDagger.DeactivateWeapon();
                _hasAttacked = false;
            }
        }
    }
}