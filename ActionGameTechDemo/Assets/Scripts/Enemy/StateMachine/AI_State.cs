using System.Linq;
using UnityEngine;

public enum AIStateType
{
    Idle = 1,
    Hurt = 2,
    Action = 3
}

public abstract class AI_State
{
    protected Animator _animator;
    protected Transform _transform;

    protected EnemyController _myController;

    protected string _lastAction;
    protected bool _stateActive;
    protected float _timeSinceStateEnter;
    protected float _animationDuration;

    // Treat states as action by default. Only Idle or Hurt states should explicitly reassign this field
    protected AIStateType _stateType = AIStateType.Action;

    public AI_State(EnemyController myController)
    {
        _myController = myController;
        _animator = _myController.GetComponent<Animator>();
        _transform = _myController.transform;
    }

    public virtual void OnStateEnter(string fromAction)
    {
        _lastAction = fromAction;

        _timeSinceStateEnter = Time.time;
        _stateActive = true;

        UnityEngine.Debug.Log($"TEST ---- DRAGON: Entered {this.GetType().Name} state from {fromAction}");
    }

    public abstract void Update(float delta);

    public virtual void OnStateExit(string toAction)
    {
        UnityEngine.Debug.Log($"TEST ---- DRAGON: Exiting {this.GetType().Name} state to {toAction}");
    }

    protected void MoveState(string nextState)
    {
        if (_stateActive == false) return;

        _stateActive = false;
        _myController.MoveToState(nextState);
    }

    protected void PlayAnimationState(string animationState)
    {
        _animator.CrossFade(animationState, 0.2f);
    }

    protected bool IsAnimationCompleted(string animationState)
    {
        var duration = _myController.GetStateDuration(animationState);
        if (Time.time - _timeSinceStateEnter > duration)
        {
            return true;
        }

        return false;
    }

    protected int GetTimesRecentlyExecuted(string actionName)
    {
        var actionHistory = _myController.GetActionHistory();
        return actionHistory.Where(e => e.Equals(actionName)).Count();
    }

    public AIStateType GetStateType()
    {
        return _stateType;
    }
}
