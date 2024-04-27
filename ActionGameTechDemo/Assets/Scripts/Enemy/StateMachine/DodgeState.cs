using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : AI_State
{
    [SerializeField]
    private float _delayBeforeMoving;

    private float _dodgeTime = 0f;

    public DodgeState(EnemyController myController) : base(myController)
    {
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);
        _dodgeTime = 0f;

        // Go to dodge anim state
    }

    public override void Update(float delta)
    {
        _dodgeTime += delta;
        if (_dodgeTime > _delayBeforeMoving)
        {
            DodgeBackwards();
        }
    }

    public override void OnStateExit(string toAction) { }

    private void DodgeBackwards()
    {
        _myController.RB.velocity = -1 * _transform.forward * _myController.ChaseSpeed * 2.5f;
    }
}
