using System;
using UnityEngine;

namespace AI.Dragon
{
    public class DiveBombState : AI_State
    {
        private string _animationState = "Divebomb";
        private float _preDiveRotationDelay = 0.8f;

        private Quaternion _targetRotationToPlayer;
        private Vector3 _divebombDirection;

        private float _trackingMultiplier = 1.6f;
        private float _damage = 120f;

        public DiveBombState(EnemyController myController) : base(myController)
        {
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            WaitOnActionCompleted();

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            GetRotationToTarget();

            if (Time.time - _timeAtStateEnter < _preDiveRotationDelay)
            {
                RotateToPlayer(_trackingMultiplier);
                _myController.RB.velocity = _myController.transform.forward * _myController.ChaseSpeed * 0.5f;
            }
            else
            {
                RotateToPlayer(0.3f); // small tracking
                ActiveAttack();
                _myController.RB.velocity = _myController.transform.forward * _myController.ChaseSpeed * 3f;
            }

            // Approximate crash behaviour. If y < -3, then it definitely has hit ground level
            if (HasCrashed())
            {
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction)
        {
            _myController.RB.velocity = Vector3.zero;
            _myController.ToggleGravity(true);
            DeactiveAttack();
            SetActionCompleted();
        }

        private bool HasCrashed()
        {
            var colliders = Physics.OverlapSphere(_myController.Bite.transform.position, 1f);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player")
                    || collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
                {
                    return true;
                }
            }

            return false;
        }

        private void GetRotationToTarget()
        {
            Vector3 targetDir = (_myController.TargetTransform.position -
                _myController.ActualBodyTransform.position).normalized;

            _targetRotationToPlayer = Quaternion.LookRotation(targetDir);
        }

        private bool RotateToPlayer(float trackingMultiplier)
        {
            var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotationToPlayer,
                _myController.TurnSpeed * Time.deltaTime * trackingMultiplier).eulerAngles;

            if (rotateVector.magnitude > 0.5f)
            {
                _transform.localEulerAngles = rotateVector;

                return true;
            }

            return false;
        }

        private void ActiveAttack()
        {
            _myController.Bite.GetComponent<SphereCollider>().radius = 120f;
            _myController.Bite.ActivateWeapon(_damage);
        }

        private void DeactiveAttack()
        {
            _myController.Bite.GetComponent<SphereCollider>().radius = 72f;
            _myController.Bite.DeactivateWeapon();
        }
    }
}
