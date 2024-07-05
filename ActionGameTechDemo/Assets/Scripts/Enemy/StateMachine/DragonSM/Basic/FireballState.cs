using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{
    public class FireballState : AI_State
    {
        protected float _delayBeforeShooting = 0.5f;

        private float _fireballTime = 0f;
        private bool _hasShot;
        private bool _isHoming;

        private Quaternion _targetRotationToPlayer;

        protected string _animationState;
        protected string _groundedAnimationState = "Fireball";
        protected string _aerialAnimationState = "BackstepFireball";
        protected string _fireballFinishState;

        public FireballState(EnemyController myController, bool isAerial) : base(myController)
        {
            _isHoming = isAerial;
            _animationState = isAerial ? _aerialAnimationState : _groundedAnimationState;

            _fireballFinishState = "Idle";
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _fireballTime = 0f;
            _hasShot = false;

            _myController.RB.velocity *= 0.3f;  // Reduce any existing velocity when firing fireball
            GetRotationToTarget();
            PlayAnimationState(_animationState);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            _fireballTime += delta;
            if (!_hasShot && _fireballTime > _delayBeforeShooting)
            {
                ShootFireball();
                _hasShot = true;
            }
            else if (!_hasShot)
            {
                RotateToPlayer();
            }

            if (IsAnimationCompleted(_animationState))
            {
                MoveState(_fireballFinishState);
            }
        }

        public override void OnStateExit(string toAction) { }

        protected virtual void ShootFireball()
        {
            ((DragonController)_myController).Fireball.SpawnFireball(_myController.TargetTransform, _isHoming);
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
    }
}
