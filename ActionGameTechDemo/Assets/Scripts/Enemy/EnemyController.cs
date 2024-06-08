using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// TO DO: Refactor this
public class EnemyController : CharacterManager
{
    [Header("Basic Stats")]
    public string EnemyName;

    public float PlayerDetectionRange;
    public Transform ActualBodyTransform;   // Transform of this Enemy's actual body (not the parent GO)
    public Transform TargetTransform;       // Transform of the target (player)
    public Rigidbody RB;

    public bool applyGravity = true;
    public float CharacterGravity = 50;

    public float MaxDistance = 50;
    public float MinDistance = 5f;
    public float ChaseSpeed = 6f;
    public float TurnSpeed = 2f;

    private EnemyHealth _enemyHealth;

    private List<Collider> _activeColliders = new List<Collider>();
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
    [Header("Stun Variables")]
    private float _damageTakenCombo = 0f;
    private float _lastDamageTakenTime = 0f;
    private float _currentStunThreshold;
    private float _lastStunTime = 0f;
    public float RestunBaseThreshold = 50;
    public float RestunThresholdGain = 150;
    public float StunResetDuration = 30f;

    /// <summary>
    /// This value starts at 100% and increases each time RestunThresholdGain is applied
    /// </summary>
    public float CurrentStunThresholdPercentage => _currentStunThreshold / RestunBaseThreshold;

    private bool isInHitStun = false;
    private Coroutine _hitStunCoroutine = null;

    public Vector3 SpawnPoint;
    private List<GameObject> _allPossiblePlayers = new List<GameObject>();

    public virtual void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyAnimator = GetComponent<Animator>();
        RB.useGravity = false;  // Always use custom gravity

        _verticalHash = Animator.StringToHash("Vertical");
        _horizontalHash = Animator.StringToHash("Horizontal");

        _originalEnemyScale = transform.localScale;
        _currentStunThreshold = RestunBaseThreshold;

        _stateInfo = StateInfoMapResolver.GetStateInfoMap(_enemyAnimator.runtimeAnimatorController.name);

        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            if (collider.enabled && collider.isTrigger == false)
            {
                _activeColliders.Add(collider);
            }
        }

        SpawnPoint = transform.position;
        // Expensive behaviour, but only on spawn. All enemies will only engage behaviour if ANY
        // player reaches within it's detection range.
        foreach (var player in FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID))
        {
            _allPossiblePlayers.Add(player.gameObject);
        }
    }

    public void OnEnable()
    {
        isInHitStun = false;
        MoveToState("Idle");
    }

    public void Update()
    {
        if (_aiState != null)
        {
            _aiState.Update(Time.deltaTime, isInHitStun);
        }

        if (isInHitStun) return;

        // If enough time has passed since the last stun, reset hit shield and stun thresholds
        if (_lastStunTime > 0.05f && Time.time - _lastStunTime > StunResetDuration)
        {
            _lastStunTime = 0f;
            _lastDamageTakenTime = 0f;
            _currentStunThreshold = RestunBaseThreshold;
            _damageTakenCombo = 0;
        }

        // Lose 5 damageCombo per second
        _damageTakenCombo = (_damageTakenCombo > 0f) ? 
            _damageTakenCombo - Time.deltaTime * 5 : 0f;
    }

    public void FixedUpdate()
    {
        if (applyGravity)
        {
            RB.velocity += (Vector3.down * CharacterGravity) * Time.fixedDeltaTime;
        }  
    }

    public void MoveToState(string targetAnimation)
    {
        LastPerformedAction = CurrentAction;
        CurrentAction = targetAnimation;

        _aiState?.OnStateExit(CurrentAction);

        _aiState = new AIStateFactory(this).GetAIStateByName(EnemyName, targetAnimation, _enemyHealth.HealthPercentage);

        _aiState?.OnStateEnter(LastPerformedAction);

        if (_aiState == null) { Debug.LogWarning($"EnemyController {EnemyName} has lost its AI State"); return; }

        // If not an idle or hurt state, store into action history
        if (_aiState.GetStateType() != AIStateType.Idle && _aiState.GetStateType() != AIStateType.Hurt)
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

    public override void TriggerHitStop(float damageAmount, bool isAttacker)
    {
        // Attacker has slightly less HitStop than Victim
        var duration = damageAmount / (isAttacker ? 300 : 500);

        if (_hitStunCoroutine != null)
        {
            StopCoroutine(_hitStunCoroutine);
        }
        _hitStunCoroutine = StartCoroutine(HitStunTimer(duration));
    }

    private IEnumerator HitStunTimer(float duration)
    {
        isInHitStun = true;
        _enemyAnimator.speed = 0f;
        RB.isKinematic = true;

        yield return new WaitForSecondsRealtime(duration);

        isInHitStun = false;
        _enemyAnimator.speed = 1f;
        RB.isKinematic = false;
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

            Debug.Log($"TEST ------ {_currentStunThreshold}");

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

    public void Die()
    {
        // And move to hurt state
        MoveToState("Die");

        // Destroy object after X seconds
        var rootCharacterGP = PrefabUtility.GetOutermostPrefabInstanceRoot(this);
        Destroy(rootCharacterGP, 0.3f);
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

    public void ToggleGravity(bool isEnabled)
    {
        applyGravity = isEnabled;
    }

    public void ToggleBossCollision(bool canCollide)
    {
        foreach (var collider in _activeColliders)
        {
            collider.isTrigger = !canCollide;
        }
    }

    public void EnableInvuln()
    {
        _enemyHealth.SetInvuln();
    }

    public void DisableInvuln()
    {
        _enemyHealth.RemoveInvuln();
    }

    public bool IsAnyPlayerNearby()
    {
        foreach (var player in _allPossiblePlayers)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < PlayerDetectionRange)
            {
                return true;
            }
        }

        return false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinDistance);
        Gizmos.DrawWireSphere(transform.position, MaxDistance);

        if (TargetTransform != null)
            Gizmos.DrawLine(ActualBodyTransform.position, TargetTransform.position);
    }
}
