using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInteractableAfterDelay: StateMachineBehaviour
{
    public string ActionName;

    [Range(0f, 1f)]
    public float CompletionRateTillSet = 1f;

    private bool _alreadyTriggered;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _alreadyTriggered = false;

        if (CheckIfAnimCompleted(animator, layerIndex))
        {
            _alreadyTriggered = true;
            animator.SetBool("IsInteracting", false);
        } 
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (CheckIfAnimCompleted(animator, layerIndex))
        {
            _alreadyTriggered = true;
            animator.SetBool("IsInteracting", false);
        }
    }

    protected bool CheckIfAnimCompleted(Animator animator, int layerIndex)
    {
        var curState = animator.GetCurrentAnimatorStateInfo(layerIndex);
        var nextState = animator.GetNextAnimatorStateInfo(layerIndex);
        var shortNameHash = Animator.StringToHash(ActionName);

        if (curState.shortNameHash == shortNameHash && curState.normalizedTime >= CompletionRateTillSet && !_alreadyTriggered)
        {
            return true;
        }
        else if (nextState.shortNameHash == shortNameHash && nextState.normalizedTime >= CompletionRateTillSet && !_alreadyTriggered)
        {
            return true;
        }
        return false;
    }
}
