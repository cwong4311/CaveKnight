using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Queue<string> _aiActionHistory = new Queue<string>();
    public string LastPerformedAction = null;
    public string CurrentAction;

    private Vector3 _originalEnemyScale;
    private IStateInfoMap _stateInfo;

    // Stunlock thresholds
    private bool _hitArmour = false;
    private float _damageTakenCombo = 0f;
    private float _currentStunThreshold;
    private float _lastStunTime = 0f;
    public float RestunBaseThreshold = 150;
    public float RestunThresholdGain = 150;
    public float StunResetDuration = 20f;

    public void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
        _verticalHash = Animator.StringToHash("Vertical");
        _horizontalHash = Animator.StringToHash("Horizontal");

        _originalEnemyScale = transform.localScale;
        _currentStunThreshold = RestunBaseThreshold;

        _stateInfo = StateInfoMapResolver.GetStateInfoMap(_enemyAnimator.runtimeAnimatorController.name);
    }

    public void OnEnable()
    {
        MoveToState("Idle");
    }

    public void Update()
    {
        if (_aiState != null)
        {
            _aiState.Update(Time.deltaTime);
        }

        // If enough time has passed since the last stun, reset hit shield and stun thresholds
        if (_lastStunTime > 0.05f && Time.time - _lastStunTime > StunResetDuration)
        {
            _lastStunTime = 0f;
            _hitArmour = false;
            _currentStunThreshold = RestunBaseThreshold;
            _damageTakenCombo = 0;
        }
    }

    public void MoveToState(string targetAnimation)
    {

        LastPerformedAction = CurrentAction;
        CurrentAction = targetAnimation;

        _aiState?.OnStateExit(CurrentAction);

        _aiState = new AIStateFactory(this).GetAIStateByName(targetAnimation);

        _aiState?.OnStateEnter(LastPerformedAction);

        // Store into action history
        if (_aiState.GetType() != typeof(IdleState))
        {
            _aiActionHistory.Enqueue(targetAnimation);
            if (_aiActionHistory.Count > 3) _aiActionHistory.Dequeue();
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

    public void SpawnFireball(Transform target)
    {
        Fireball.SpawnFireball(target);
    }

    public void GetHit(float damageTaken)
    {
        // If not yet hit, move into hurt state
        if (!_hitArmour)
        {
            MoveToState("Hurt");

            _hitArmour = true;
            _damageTakenCombo = 0;

            _lastStunTime = Time.time;
        }
        // Otherwise, if more damage is dealt while already in hurt state
        // Trigger additional effects
        else
        {
            _damageTakenCombo += damageTaken;
            if (_damageTakenCombo > _currentStunThreshold)
            {
                // Reset damage counter, and increase threshold
                _damageTakenCombo = 0;
                _currentStunThreshold += RestunThresholdGain;

                // Reset state duration
                MoveToState("Hurt");

                _lastStunTime = Time.time;
            }
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

    public string GetLatestAction()
    {
        return _aiActionHistory?.LastOrDefault();
    }

    public void FlipEnemyScale()
    {
        transform.localScale = new Vector3(-_originalEnemyScale.x, _originalEnemyScale.y, _originalEnemyScale.z);
    }

    public void RestoreEnemyScale()
    {
        if (transform.localScale != _originalEnemyScale)
        {
            transform.localScale = _originalEnemyScale;
        }  
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinDistance);
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
    }
}
