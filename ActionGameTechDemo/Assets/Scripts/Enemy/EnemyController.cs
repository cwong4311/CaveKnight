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

    private EnemyHealth _enemyHealth;

    private Animator _enemyAnimator;
    private Dictionary<string, float> _enemyAnimatorInfoMap = new Dictionary<string, float>();
    private int _verticalHash;
    private int _horizontalHash;

    private AI_State _aiState;
    private List<string> _aiActionHistory = new List<string>();
    public string LastPerformedAction = null;
    public string CurrentAction;

    private Vector3 _originalEnemyScale;
    private IStateInfoMap _stateInfo;

    // Stunlock thresholds
    private float _damageTakenCombo = 0f;
    private float _lastDamageTakenTime = 0f;
    private float _currentStunThreshold;
    private float _lastStunTime = 0f;
    public float RestunBaseThreshold = 50;
    public float RestunThresholdGain = 150;
    public float StunResetDuration = 30f;

    public void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
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
            _lastDamageTakenTime = 0f;
            _currentStunThreshold = RestunBaseThreshold;
            _damageTakenCombo = 0;
        }

        _damageTakenCombo -= Time.deltaTime * 5;    // Lose 5 damageCombo per second
    }

    public void MoveToState(string targetAnimation)
    {
        LastPerformedAction = CurrentAction;
        CurrentAction = targetAnimation;

        _aiState?.OnStateExit(CurrentAction);

        _aiState = new AIStateFactory(this).GetAIStateByName(targetAnimation);

        _aiState?.OnStateEnter(LastPerformedAction);

        // If not an idle or hurt state, store into action history
        if (_aiState.GetType() != typeof(IdleState) && _aiState.GetType() != typeof(HurtState))
        {
            _aiActionHistory.Insert(0, targetAnimation);
            if (_aiActionHistory.Count > 5) _aiActionHistory.RemoveAt(5);
        }
        // If an idle state, remove any trailing invulns
        else
        {
            _enemyHealth.RemoveInvuln();
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

    public void SpawnFireball(Transform target, bool isHoming)
    {
        Fireball.SpawnFireball(target, isHoming);
    }

    public void GetHit(float damageTaken)
    {
        // If enough damage is dealt, flinch
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

    public void ForceGetHit()
    {
        // When forced hit, reset all parameters (and set stun threshold to level 2)
        _damageTakenCombo = 0;
        _currentStunThreshold = RestunBaseThreshold + RestunThresholdGain;
        _lastStunTime = Time.time;

        // And move to hurt state
        MoveToState("Hurt");
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

    public string[] GetActionHistory()
    {
        return _aiActionHistory.ToArray();
    }
    public string GetLatestAction => _aiActionHistory[0] ?? "";

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
