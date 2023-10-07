using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private PlayerAnimationHandler _animatorHandler;

    private Vector3 _velocityDuringAnimation;

    public void Initialise()
    {
        _animatorHandler = GetComponent<PlayerAnimationHandler>();
    }

    public void LightAttack()
    {
        _animatorHandler?.PlayAnimation("LightAttack", true);
    }

    public void HeavyAttack()
    {
        _animatorHandler?.PlayAnimation("HeavyAttack", true);
    }
}
