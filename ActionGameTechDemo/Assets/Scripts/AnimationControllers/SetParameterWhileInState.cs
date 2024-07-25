using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets a parameter (bool) while in state, and unsets it when exiting state
/// </summary>
public class SetParameterWhileInState : StateMachineBehaviour
{
    public string ParameterName;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(ParameterName, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(ParameterName, false);
    }
}
