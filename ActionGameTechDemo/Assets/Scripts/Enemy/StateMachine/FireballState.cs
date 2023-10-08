using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballState : AI_State
{
    [SerializeField]
    private float _delayBeforeShooting;

    private float _fireballTime = 0f;
    private bool _hasShot;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _fireballTime = 0f;
        _hasShot = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _fireballTime += Time.deltaTime;
        if (!_hasShot && _fireballTime > _delayBeforeShooting)
        {
            ShootFireball();
            _hasShot = true;
        }
    }

    private void ShootFireball()
    {
        _enemyController.SpawnFireball(_enemyController.TargetTransform.position);
    }
}
