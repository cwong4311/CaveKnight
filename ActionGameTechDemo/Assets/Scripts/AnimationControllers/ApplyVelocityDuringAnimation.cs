using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyVelocityDuringAnimation : StateMachineBehaviour
{
    public float VelocityApplied;
    private PlayerAnimationHandler _playerAnimator;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerAnimator = animator.GetComponent<PlayerAnimationHandler>();
        ApplyVelocity();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ApplyVelocity();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StopApplyVelocity();
    }

    private void ApplyVelocity()
    {
        if (_playerAnimator != null)
        {
            _playerAnimator.ApplyVelocityDuringAnimation(VelocityApplied);
        }
    }

    private void StopApplyVelocity()
    {
        if (_playerAnimator != null)
        {
            _playerAnimator.StopApplyingVelocityDuringAnimation();
        }
    }
}
