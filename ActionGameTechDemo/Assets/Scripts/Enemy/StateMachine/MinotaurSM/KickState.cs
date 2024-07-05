using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Minotaur
{
    public class KickState : AI_State
    {
        private int _delayInFrames = 36;
        private int _untilInFrames = 62;
        public float DelayBeforeAttackActive => FramesToTime(_delayInFrames);
        public float AttackActiveUntil => FramesToTime(_untilInFrames);

        public float Damage = 40f;

        private string _animationState = "Attack_3";
        public bool _hasAttacked = false;

        private Quaternion _targetRotationToPlayer;
        private NavMeshAgent _myNavAgent;

        public KickState(EnemyController myController) : base(myController)
        {
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _myNavAgent = ((MinotaurController)_myController).NavMeshAgent;

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
                MoveForward(delta);
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

        private void MoveForward(float delta)
        {
            _myNavAgent.Move(_myController.transform.forward * _myNavAgent.speed * delta);
        }

        private void ActiveAttack()
        {
            if (_hasAttacked == false)
            {
                ((MinotaurController)_myController).Foot.ActivateWeapon(Damage);
                _hasAttacked = true;
            }
        }

        private void DeactiveAttack()
        {
            if (_hasAttacked)
            {
                ((MinotaurController)_myController).Foot.DeactivateWeapon();
                _hasAttacked = false;
            }
        }
    }
}