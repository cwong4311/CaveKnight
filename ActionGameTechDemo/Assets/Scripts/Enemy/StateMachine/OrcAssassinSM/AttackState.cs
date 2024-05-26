using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.OrcAssassin
{ 
    public class AttackState : AI_State
    {
        public float DelayBeforeAttackActive = 0.9f;
        public float AttackActiveUntil = 1.25f;
        public float Damage = 15f;

        private string _animationState = "Attack";
        public bool _hasAttacked = false;

        private Quaternion _targetRotationToPlayer;

        public AttackState(EnemyController myController) : base(myController)
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

            if (Time.time - _timeAtStateEnter >= DelayBeforeAttackActive && _hasAttacked == false)
            {
                ActiveAttack();
                _hasAttacked = true;
            }
            else if (Time.time - _timeAtStateEnter >= AttackActiveUntil)
            {
                DeactiveAttack();
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
            ((OrcAssassinController)_myController).MainDagger.ActivateWeapon(Damage);
        }

        private void DeactiveAttack()
        {
            ((OrcAssassinController)_myController).MainDagger.DeactivateWeapon();
        }
    }
}