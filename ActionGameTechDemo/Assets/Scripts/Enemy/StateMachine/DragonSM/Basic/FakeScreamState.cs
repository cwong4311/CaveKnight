using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{
    public class FakeScreamState : AI_State
    {
        private string _animationState = "Scream";

        public FakeScreamState(EnemyController myController) : base(myController)
        {
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            // Start to scream, but then get hurt trying to do so.
            // This is the Beefy boi's fake scream
            var duration = _myController.GetStateDuration(_animationState) / 4;
            if (Time.time - _timeAtStateEnter > duration)
            {
                _myController.ForceGetHit();
            }
        }

        public override void OnStateExit(string toAction) { }
    }
}
