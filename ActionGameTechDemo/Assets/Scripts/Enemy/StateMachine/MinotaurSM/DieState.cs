using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Minotaur
{
    public class DieState : AI_State
    {
        private string _animationState = "Die";

        public DieState(EnemyController myController) : base(myController)
        {
            _stateType = AIStateType.Hurt;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            _myController.TargetTransform = null;
            PlayAnimationState(_animationState);
        }

        public override void OnStateExit(string toAction) { }
    }
}
