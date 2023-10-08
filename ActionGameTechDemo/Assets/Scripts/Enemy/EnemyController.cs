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

public class EnemyController : CharacterManager
{
    public EnemyCommands[] EnemyCommands;
    public float PlayerDetectionRange;
    public Transform TargetTransform;
    public Rigidbody RB;

    public float MaxDistance = 50;
    public float MinDistance = 5f;

    public float ChaseSpeed = 6f;
    public float TurnSpeed = 2f;

    private Animator _enemyAnimator;
    private int _verticalHash;
    private int _horizontalHash;

    public string LastPerformedAction = null;
    public string CurrentAction;

    public void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
        _verticalHash = Animator.StringToHash("Vertical");
        _horizontalHash = Animator.StringToHash("Horizontal");

        CurrentAction = "Idle";
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

        _enemyAnimator.CrossFade(targetAnimation, 0.2f);
    }

    public void RegisterState(string stateName)
    {
        LastPerformedAction = CurrentAction;
        CurrentAction = stateName;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinDistance);
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
    }
}
