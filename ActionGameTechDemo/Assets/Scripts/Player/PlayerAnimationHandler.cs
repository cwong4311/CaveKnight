using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    public Animator Anim => _animator;
    public PlayerInputHandler _inputHandler;
    public PlayerController _playerController;

    private Animator _animator;
    private int _verticalHash;
    private int _horizontalHash;

    private Vector3 _velocityDuringAnimation;
    private Vector3 _lastMoveDirection;

    /// <summary>
    /// How strong do these motions track the (locked on) enemy. By default, motions never track enemy.
    /// </summary>
    public float? MotionTrackingEnemyStrength = null;

    public void Initialise()
    {
        _animator = GetComponent<Animator>();

        _verticalHash = Animator.StringToHash("Vertical");
        _horizontalHash = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimation(float vertical, float horizontal, bool isSprinting)
    {
        float clampVertical = vertical switch
        {
            > 0.55f => 1f,
            > 0f => 0.5f,
            < -0.55f => -1f,
            < 0f => -0.5f,
            _ => 0f
        };
        
        float clampHorizontal = horizontal switch
        {
            > 0.55f => 1f,
            > 0f => 0.5f,
            < -0.55f => -1f,
            < 0f => -0.5f,
            _ => 0f
        };

        if (isSprinting)
        {
            clampVertical = clampVertical switch
            {
                > 0 => 2f,
                < 0 => -2f,
                _ => 0f
            };

            clampHorizontal = clampHorizontal switch
            {
                > 0 => 2f,
                < 0 => -2f,
                _ => 0f
            };
        }

        _animator.SetFloat(_verticalHash, clampVertical, 0.1f, Time.deltaTime);
        _animator.SetFloat(_horizontalHash, clampHorizontal, 0.1f, Time.deltaTime);
    }

    public void PlayAnimation(string targetAnimation, bool isInteracting)
    {
        _animator.applyRootMotion = isInteracting;
        _animator.SetBool("IsInteracting", isInteracting);
        _animator.CrossFade(targetAnimation, 0.2f);

        _velocityDuringAnimation = Vector3.zero;
    }

    public void ToggleBlocking(bool isBlocking)
    {
        _animator.SetBool("IsBlocking", isBlocking);
    }

    private void OnAnimatorMove()
    {
        if (_inputHandler.IsInteracting == false) return;
        if (Mathf.Approximately(Time.deltaTime, 0f)) return;

        float delta = Time.deltaTime;
        _playerController.RB.drag = 0;

        Vector3 deltaPosition = Anim.deltaPosition;
        deltaPosition.y = 0;
        var velocity = deltaPosition / delta;
        _playerController.RB.velocity = (velocity + _velocityDuringAnimation);
    }

    public void ApplyVelocityDuringAnimation(float velocity)
    {
        if (_inputHandler.IsInteracting == false) return;

        var _moveDirection = _lastMoveDirection.normalized * velocity;
        _moveDirection.y = 0f;

        // Gets applied during OnAnimatorMove
        _velocityDuringAnimation = Vector3.ProjectOnPlane(_moveDirection, Vector3.zero);
    }

    public void ApplyForwardVelocityDuringAnimation(float velocity)
    {
        if (_inputHandler.IsInteracting == false) return;

        var _moveDirection = transform.forward.normalized * velocity;
        _moveDirection.y = 0f;

        // Gets applied during OnAnimatorMove
        _velocityDuringAnimation = Vector3.ProjectOnPlane(_moveDirection, Vector3.zero);
    }

    public void StopApplyingVelocityDuringAnimation()
    {
        _velocityDuringAnimation = Vector3.zero;
    }

    public void UpdateMovementDirection(Vector3 direction)
    {
        _lastMoveDirection = direction;
    }

    public void StartMotionTrackingEnemy(float strength)
    {
        MotionTrackingEnemyStrength = strength;
    }

    public void StopMotionTrackingEnemy()
    {
        MotionTrackingEnemyStrength = null;
    }
}
