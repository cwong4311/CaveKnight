using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyCommands
{
    public string commandName;
    public bool isAttack;
    public float attackDamage;
}

// TO DO: Refactor this
public class EnemyController : CharacterManager
{
    public EnemyCommands[] EnemyCommands;
    public float PlayerDetectionRange;
    public Transform TargetTransform;
    public Rigidbody RB;
    public WeaponDamager Bite;
    public WeaponDamager Tail;
    public EnemyFireball Fireball;

    public float MaxDistance = 50;
    public float MinDistance = 5f;
    public float ChaseSpeed = 6f;
    public float TurnSpeed = 2f;

    private Animator _enemyAnimator;
    private Dictionary<string, float> _enemyAnimatorInfoMap = new Dictionary<string, float>();
    private int _verticalHash;
    private int _horizontalHash;

    private AI_State _aiState;
    public string LastPerformedAction = null;
    public string CurrentAction;

    private bool _hitArmour;
    private IStateInfoMap _stateInfo;

    public void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
        _verticalHash = Animator.StringToHash("Vertical");
        _horizontalHash = Animator.StringToHash("Horizontal");

        _stateInfo = StateInfoMapResolver.GetStateInfoMap(_enemyAnimator.runtimeAnimatorController.name);

        _aiState = new IdleState(this);
        _aiState.OnStateEnter(null);
    }

    public void Update()
    {
        if (_aiState != null)
        {
            _aiState.Update(Time.deltaTime);
        }
    }

    public void UpdateMovementParameters(float vertical, float horizontal)
    {
        UpdateMovementParameters(vertical, horizontal, true);
    }

    public void UpdateMovementParameters(float vertical, float horizontal, bool useSmoothDamp)
    {
        if (useSmoothDamp)
        {
            _enemyAnimator.SetFloat(_verticalHash, vertical, 0.1f, Time.deltaTime);
            _enemyAnimator.SetFloat(_horizontalHash, horizontal, 0.1f, Time.deltaTime);
        }
        else
        {
            _enemyAnimator.SetFloat(_verticalHash, vertical);
            _enemyAnimator.SetFloat(_horizontalHash, horizontal);
        }
    }

    public void MoveToState(string targetAnimation)
    {
        LastPerformedAction = CurrentAction;
        CurrentAction = targetAnimation;

        _hitArmour = false;
        _aiState = new AIStateFactory(this).GetAIStateByName(targetAnimation);
    }

    public void RegisterState(string stateName)
    {
        LastPerformedAction = CurrentAction;
        CurrentAction = stateName;
    }

    public void SpawnFireball(Transform target)
    {
        Fireball.SpawnFireball(target);
    }

    public void GetHit()
    {
        if (CurrentAction == "Idle" && !_hitArmour)
        {
            _enemyAnimator.CrossFade("Get Hit", 0.2f);
            _hitArmour = true;
        }
    }

    public float? GetStateDuration(string stateName)
    {
        var stateInfo = _stateInfo.GetStateInfoByName(stateName);
        if (stateInfo.HasValue)
        {
            return (float)stateInfo.Value.Duration;
        }
        return null;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinDistance);
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
    }
}
