using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{ 
    public class AttackState : AI_State
    {
        public float DelayBeforeAttackActive = 0.3f;
        public float Damage = 20f;

        private bool isPrimaryAttack;
        private string _primaryAttack = "Basic Attack";
        private string _secondaryAttack = "Tail Attack";

        private string _animationState;
        private Quaternion _targetRotationToPlayer;
        private TargetDirection _directionOfPlayer;

        private Vector3 _bossScale;

        public AttackState(EnemyController myController) : base(myController)
        {
            _directionOfPlayer = DirectionOfTarget.GetDirectionOfTarget(_myController.transform, _myController.TargetTransform);
            isPrimaryAttack = (_directionOfPlayer == TargetDirection.Forward) ? true : false;

            _bossScale = myController.transform.localScale;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _animationState = (isPrimaryAttack) ? _primaryAttack : _secondaryAttack;

            if (isPrimaryAttack)
                GetRotationToTarget();

            if (!isPrimaryAttack && _directionOfPlayer == TargetDirection.Right)
                _myController.FlipEnemyScale();

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta)
        {
            if (isPrimaryAttack)
                RotateToPlayer();

            if (Time.time - _timeAtStateEnter >= DelayBeforeAttackActive)
            {
                ActiveAttack();
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
            if (isPrimaryAttack)
            {
                _myController.Bite.ActivateWeapon(Damage);
            }
            else
            {
                _myController.Tail.ActivateWeapon(Damage);
            }
        }

        private void DeactiveAttack()
        {
            _myController.Bite.DeactivateWeapon();
            _myController.Tail.DeactivateWeapon();
        }
    }
}