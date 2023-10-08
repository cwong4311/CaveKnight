using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_State : StateMachineBehaviour
{
    // TO DO: Refactor to actual StateMachine (code)
    public string[] UsableCommands;

    protected EnemyController _enemyController;
    protected Animator _animator;
    protected Transform _transform;

    protected bool _stateActive;
    protected float _timeSinceStateEnter;
    protected string _previousAction = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _enemyController = animator.GetComponent<EnemyController>();
        _animator = animator;
        _transform = animator.transform;
        
        _timeSinceStateEnter = Time.time;
        _stateActive = true;
    }

    protected void MoveState(string nextState)
    {
        if (_stateActive == false) return;

        _stateActive = false;
        _enemyController.MoveToState(nextState);
    }
}
