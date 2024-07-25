using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.OrcAssassin
{ 
    public class FuryAttackState : AI_State
    {
        private int _hit1Delay = 20;
        private int _hit1Until = 30;
        private int _hit2Delay = 50;
        private int _hit2Until = 60;
        private int _hit3Delay = 100;
        private int _hit3Until = 110;

        public float Hit1Damage = 10f;
        public float Hit2Damage = 10f;
        public float Hit3Damage = 30f;
        public float Hit1Delay => FramesToTime(_hit1Delay);
        public float Hit1Until => FramesToTime(_hit1Until);
        public float Hit2Delay => FramesToTime(_hit2Delay);
        public float Hit2Until => FramesToTime(_hit2Until);
        public float Hit3Delay => FramesToTime(_hit3Delay);
        public float Hit3Until => FramesToTime(_hit3Until);


        private int _attackStep;
        public bool _hasAttacked = false;

        private string _firstAttackAnimState = "SpecialAttack";
        private string _secondAttackAnimState = "SpecialAttack2";

        private float _firstAttackDuration;
        private float _secondAttackDuration;
        private int _animStep;

        public FuryAttackState(EnemyController myController) : base(myController)
        {
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _myController.RB.velocity *= 0.3f;

            _attackStep = 1;
            _animStep = 1;

            _firstAttackDuration = _myController.GetStateDuration(_firstAttackAnimState) ?? 0;
            _secondAttackDuration = _firstAttackDuration + (_myController.GetStateDuration(_secondAttackAnimState) ?? 0);

            PlayAnimationState(_firstAttackAnimState, 0.05f);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            if (_attackStep == 1)
            {
                RotateToPlayer();

                if (Time.time - _timeAtStateEnter >= Hit1Delay && _hasAttacked == false)
                {
                    ActiveAttack(Hit1Damage);
                }
                else if (Time.time - _timeAtStateEnter >= Hit1Until)
                {
                    DeactiveAttack();
                    _attackStep = 2;
                }
            }
            else if (_attackStep == 2)
            {
                RotateToPlayer();

                if (Time.time - _timeAtStateEnter >= Hit2Delay && _hasAttacked == false)
                {
                    ActiveAttack(Hit2Damage);
                }
                else if (Time.time - _timeAtStateEnter >= Hit2Until)
                {
                    DeactiveAttack();
                    _attackStep = 3;
                }
            }
            else if (_attackStep == 3)
            {
                RotateToPlayer();

                if (Time.time - _timeAtStateEnter >= Hit3Delay && _hasAttacked == false)
                {
                    ActiveAttack(Hit3Damage);
                }
                else if (Time.time - _timeAtStateEnter >= Hit3Until)
                {
                    DeactiveAttack();
                    _attackStep = 4;
                }
            }

            if (Time.time - _timeAtStateEnter > _firstAttackDuration && _animStep == 1)
            {
                _animStep = 2;
                PlayAnimationState(_secondAttackAnimState, 0.25f);
            }
            else if (Time.time - _timeAtStateEnter > _secondAttackDuration && _animStep == 2)
            {
                _animStep = 3;
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction)
        {
            DeactiveAttack();
            _hasAttacked = false;
        }

        private bool RotateToPlayer()
        {
            Vector3 targetDir = (_myController.TargetTransform.position - _transform.position).normalized;
            var targetRotationToPlayer = Quaternion.LookRotation(targetDir);

            var rotateVector = Quaternion.Slerp(_transform.rotation, targetRotationToPlayer, _myController.TurnSpeed * Time.deltaTime).eulerAngles;

            if (rotateVector.magnitude > 0.5f)
            {
                _transform.localEulerAngles = Vector3.up * rotateVector.y;

                return true;
            }

            return false;
        }

        private void ActiveAttack(float damage)
        {
            if (_hasAttacked == false)
            {
                ((OrcAssassinController)_myController).MainDagger.ActivateWeapon(damage);
                _hasAttacked = true;
            }
        }

        private void DeactiveAttack()
        {
            if (_hasAttacked == true)
            {
                ((OrcAssassinController)_myController).MainDagger.DeactivateWeapon();
                _hasAttacked = false;
            }
        }
    }
}