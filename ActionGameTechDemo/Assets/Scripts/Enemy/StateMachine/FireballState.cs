using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballState : AI_State
{
    private float _delayBeforeShooting = 0.5f;

    private float _fireballTime = 0f;
    private bool _hasShot;

    private string _animationState;
    private string _groundedAnimationState = "Fireball";
    private string _aerialAnimationState = "BackstepFireball";

    public FireballState(EnemyController myController, bool isAerial) : base(myController)
    {
        _animationState = isAerial ? _aerialAnimationState : _groundedAnimationState;
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);

        _fireballTime = 0f;
        _hasShot = false;

        PlayAnimationState(_animationState);
    }

    public override void Update(float delta)
    {
        _fireballTime += delta;
        if (!_hasShot && _fireballTime > _delayBeforeShooting)
        {
            ShootFireball();
            _hasShot = true;
        }

        if (IsAnimationCompleted(_animationState))
        {
            MoveState("Idle");
        }
    }

    public override void OnStateExit(string toAction) { }

    private void ShootFireball()
    {
        _myController.SpawnFireball(_myController.TargetTransform);
    }
}
