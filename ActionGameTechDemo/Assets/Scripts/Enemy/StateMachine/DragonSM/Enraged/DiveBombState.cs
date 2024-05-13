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

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta)
        {
            if (Time.time - _timeAtStateEnter < _preDiveRotationDelay)
            {
                GetRotationToTarget();
                RotateToPlayer();
                _myController.RB.velocity = _myController.transform.forward * _myController.ChaseSpeed * 0.5f;
            }
            else
            {
                ActiveAttack();
                _myController.RB.velocity = _myController.transform.forward * _myController.ChaseSpeed * 4f;
            }

            //TODO: Modify Crash detection behaviour. When bite collider hits ground
            if (IsAnimationCompleted(_animationState))
            {
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction)
        {
            _myController.RB.velocity = Vector3.zero;
            _myController.ToggleGravity(true);
            DeactiveAttack();
        }

        private void GetRotationToTarget()
        {
            Vector3 targetDir = (_myController.TargetTransform.position - _myController.Bite.transform.position).normalized;
            _targetRotationToPlayer = Quaternion.LookRotation(targetDir);

            Debug.Log("TEST --- " + _targetRotationToPlayer.eulerAngles);
        }

        private bool RotateToPlayer()
        {
            var rotateVector = Quaternion.Slerp(_transform.rotation, _targetRotationToPlayer,
                _myController.TurnSpeed * Time.deltaTime * _trackingMultiplier).eulerAngles;

            if (rotateVector.magnitude > 0.5f)
            {
                _transform.localEulerAngles = rotateVector;

                return true;
            }

            return false;
        }

        private void ActiveAttack()
        {
            _myController.Bite.ActivateWeapon(_damage);
        }

        private void DeactiveAttack()
        {
            _myController.Bite.DeactivateWeapon();
        }
    }
}
