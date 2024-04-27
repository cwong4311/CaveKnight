using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_State
{
    // TO DO: Refactor to actual StateMachine (code)
    public string[] UsableCommands;

    protected Animator _animator;
    protected Transform _transform;

    protected EnemyController _myController;

    protected string _lastAction;
    protected bool _stateActive;
    protected float _timeSinceStateEnter;

    public AI_State(EnemyController myController)
    {
        _myController = myController;
    }

    public virtual void OnStateEnter(string fromAction)
    {
        _animator = _myController.GetComponent<Animator>();
        _transform = _myController.transform;

        _lastAction = fromAction;

        _timeSinceStateEnter = Time.time;
        _stateActive = true;
    }

    public abstract void Update(float delta);

    public abstract void OnStateExit(string toAction);

    protected void MoveState(string nextState)
    {
        if (_stateActive == false) return;

        _stateActive = false;
        _myController.MoveToState(nextState);
    }
}
