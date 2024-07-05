using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.OrcAssassin
{
    public class DodgeState : AI_State
    {
        private float _delayBeforeMoving = 0.05f;

        private float _dodgeTime;

        private string _backstepAnimationState = "Dodge";

        private Vector3? _dodgeVelocity;
        private NavMeshAgent _myNavAgent;

        public DodgeState(EnemyController myController) : base(myController)
        {
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _dodgeTime = 0f;
            _dodgeVelocity = null;
            _myNavAgent = ((OrcAssassinController)_myController).NavMeshAgent;

            // Go to dodge anim state
            PlayAnimationState(_backstepAnimationState);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            _dodgeTime += delta;
            if (_dodgeTime > _delayBeforeMoving)
            {
                if (_dodgeVelocity.HasValue == false)
                {
                    _dodgeVelocity = GetDodgeDirection();
                    _myController.EnableInvuln();
                    _myController.ToggleBossCollision(false);
                }

                DodgeInDirection(_dodgeVelocity.Value, delta);
            }

            if (IsAnimationCompleted(_backstepAnimationState))
            {
                _myController.DisableInvuln();
                _myController.ToggleBossCollision(true);
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction)
        {
            _myController.DisableInvuln();
            _myController.ToggleBossCollision(true);
        }

        private void DodgeInDirection(Vector3 dodgeVelocity, float delta)
        {
            _myNavAgent.Move(dodgeVelocity * delta);
        }

        private Vector3 GetDodgeDirection()
        {
            Vector3 dodgeVelocity = Vector3.zero;

            // Raycast forward 10f. If it hits, not enough position. Dodge forwards
            Ray backwardRay = new Ray(_transform.position, _transform.forward * -1);
            if (Physics.Raycast(backwardRay, out var hit, 10f, LayerMask.GetMask("Environment")))
            {
                dodgeVelocity = _transform.forward * _myController.ChaseSpeed;
            }
            // If it doesn't hit, enough space. Dodge backwards
            else
            {
                dodgeVelocity = -1 * _transform.forward * _myController.ChaseSpeed;
            }

            return dodgeVelocity;
        }
    }
}
