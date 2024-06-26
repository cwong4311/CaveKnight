using System;
using System.Collections;
using UnityEngine;

namespace AI.Dragon
{
    public class DiveBombState : AI_State
    {
        private string _animationState = "Divebomb";
        private float _preDiveRotationDelay = 0.3f;

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
            _myController.ToggleBossCollision(false);

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            GetRotationToTarget();

            if (Time.time - _timeAtStateEnter < _preDiveRotationDelay)
            {
                RotateToPlayer(_trackingMultiplier);
                _myController.RB.velocity = _myController.transform.forward * _myController.ChaseSpeed * 0.3f;
            }
            else
            {
                RotateToPlayer(0.3f); // small tracking
                ActiveAttack();
                _myController.RB.velocity = _myController.transform.forward * _myController.ChaseSpeed * 3.5f;
            }

            if (HasCrashed())
            {
                ResetXRotation();
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction)
        {
            _myController.StartCoroutine(ReturnToIdlePosition());

            _myController.RB.velocity *= 0.3f;
            _myController.ToggleGravity(true);
            _myController.ToggleBossCollision(true);
            SetActionCompleted();
            DeactiveAttack();
        }

        private bool HasCrashed()
        {
            var colliders = Physics.OverlapSphere(((DragonController)_myController).Bite.transform.position, 1f);
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
            var adjustedTarget = _myController.TargetTransform.position + _myController.TargetTransform.up;
            Vector3 targetDir = (adjustedTarget - _myController.ActualBodyTransform.position).normalized;

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

        private void ResetXRotation()
        {
            var resetRotation = _myController.transform.rotation.eulerAngles;
            resetRotation.x = 0;
            _myController.transform.localEulerAngles = resetRotation;
        }

        private void ActiveAttack()
        {
            ((DragonController)_myController).Bite.GetComponent<SphereCollider>().radius = 120f;
            ((DragonController)_myController).Bite.ActivateWeapon(_damage);
        }

        private void DeactiveAttack()
        {
            ((DragonController)_myController).Bite.GetComponent<SphereCollider>().radius = 72f;
            ((DragonController)_myController).Bite.DeactivateWeapon();
        }

        /// <summary>
        /// Gradually reset the y position to 0, using Slerp.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReturnToIdlePosition()
        {
            var currentTimeTaken = 0f;
            var totalDuration = 0.5f;
            var endYPos = 30f;

            while (currentTimeTaken < totalDuration)
            {
                var destination = new Vector3(_myController.transform.position.x, endYPos, _myController.transform.position.z);
                _myController.transform.position = Vector3.Slerp(_myController.transform.position, destination, currentTimeTaken / totalDuration);

                currentTimeTaken += Time.deltaTime;
                yield return null;
            }

            _myController.transform.position = new Vector3(_myController.transform.position.x, endYPos, _myController.transform.position.z);
        }
    }
}
