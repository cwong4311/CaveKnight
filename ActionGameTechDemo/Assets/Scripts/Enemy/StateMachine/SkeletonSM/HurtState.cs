using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Skeleton
{
    public class HurtState : AI_State
    {
        private string _animationState = "Get Hit";

        private string _onNormalHit = "Hit";
        private string _onHeavyHit = "Stagger";

        public HurtState(EnemyController myController) : base(myController)
        {
            _stateType = AIStateType.Hurt;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            var currentStunThreshold = _myController.CurrentStunThresholdPercentage;
            if (currentStunThreshold >= 2)
            {
                _animationState = _onHeavyHit;
            }
            else
            {
                _animationState = _onNormalHit;
            }

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            if (IsAnimationCompleted(_animationState))
            {
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction) { }
    }
}
