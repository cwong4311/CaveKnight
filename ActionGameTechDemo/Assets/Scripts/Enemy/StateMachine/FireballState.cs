using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballState : AI_State
{
    [SerializeField]
    private float _delayBeforeShooting;

    private float _fireballTime = 0f;
    private bool _hasShot;

    public FireballState(EnemyController myController) : base(myController)
    {
    }

    public override void OnStateEnter(string fromAction)
    {
        base.OnStateEnter(fromAction);

        _fireballTime = 0f;
        _hasShot = false;

        // GO to FIreball State
    }

    public override void Update(float delta)
    {
        _fireballTime += delta;
        if (!_hasShot && _fireballTime > _delayBeforeShooting)
        {
            ShootFireball();
            _hasShot = true;
        }
    }

    public override void OnStateExit(string toAction) { }

    private void ShootFireball()
    {
        _myController.SpawnFireball(_myController.TargetTransform);
    }
}
