using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : AI_State
{
    private string _animationState = "Get Hit";
 
    public HurtState(EnemyController myController) : base(myController)
    {
        _stateType = AIStateType.Hurt;
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);

        PlayAnimationState(_animationState);
    }

    public override void Update(float delta)
    {
        if (IsAnimationCompleted(_animationState))
        {
            MoveState("Idle");
        }
    }

    public override void OnStateExit(string toAction) { }
}
