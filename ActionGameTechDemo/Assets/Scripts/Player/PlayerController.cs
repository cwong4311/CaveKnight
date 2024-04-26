using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterManager
{
    public Rigidbody RB;
    public CameraController CameraController;
    private GameObject _baseCam;

    public float MovementSpeed = 5f;
    public float SprintSpeed = 8f;
    public float RotationSpeed = 10f;
    public bool IsSprinting;
    public bool IsRolling;
    public bool IsPerformingAction;
    public bool IsBlocking;
    public float BlockingMovementSpeed = 4f;

    private Transform _cameraGO;
    private Vector3 _moveDirection;

    private PlayerInputHandler _inputHandler;
    private PlayerAnimationHandler _animator;
    private PlayerWeapon _weapon;
    private PlayerHealth _health;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<PlayerInputHandler>();

        _animator = GetComponentInChildren<PlayerAnimationHandler>();
        if (_animator != null)
        {
            _animator.Initialise();
        }

        _weapon = GetComponentInChildren<PlayerWeapon>();
        if (_weapon != null)
        {
            _weapon.Initialise();
        }

        _health = GetComponent<PlayerHealth>();

        _cameraGO = Camera.main.transform;
        CameraController = FindObjectOfType<CameraController>();
    }

    public void Update()
    {
        float delta = Time.deltaTime;

        _inputHandler.IsInteracting = _animator.Anim.GetBool("IsInteracting");
        IsPerformingAction = !_animator.Anim.GetBool("CanMove");

        _inputHandler.ParseInput(delta);
        IsSprinting = _inputHandler.IsSprinting;

        // Don't allow running while blocking
        IsBlocking = _inputHandler.IsBlocking;
        if (IsBlocking) IsSprinting = false;

        if (_inputHandler.IsInteracting == false && IsRolling)
        {
            IsRolling = false;
        }

        UpdateMovement(delta);
        UpdateRotation(delta);
        UpdateRollAndSprint(delta);
        UpdateAttack(delta);
    }

    private void UpdateMovement(float delta)
    {
        if (IsPerformingAction) return;

        _moveDirection = ((_cameraGO.forward * _inputHandler.VerticalMove)
            + (_cameraGO.right * _inputHandler.HorizontalMove))
            .normalized;
        _moveDirection.y = 0f;
        
        float speed = (IsSprinting && _inputHandler.FinalMovementAmount > 0.5f) ? SprintSpeed : MovementSpeed;
        speed = (IsBlocking) ? BlockingMovementSpeed : speed;
        _moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, Vector3.zero);
        RB.velocity = projectedVelocity;

        if (_inputHandler.LockedOn && !IsSprinting)
        {
            _animator.UpdateAnimation(_inputHandler.VerticalMove, _inputHandler.HorizontalMove, false);
        }
        else
        {
            _animator.UpdateAnimation(_inputHandler.FinalMovementAmount, 0f, IsSprinting);
        }

        _animator.ToggleBlocking(IsBlocking);
    }

    private void UpdateRotation(float delta)
    {
        if (_inputHandler.LockedOn)
        {
            // While locked on, special rotation behaviour while running and rolling
            if (IsSprinting || IsRolling)
            {
                Vector3 targetDir = ((CameraController.CameraTransform.forward * _inputHandler.VerticalMove)
                   + (CameraController.CameraTransform.right * _inputHandler.HorizontalMove))
                   .normalized;
                targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                Quaternion rotateVector = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * delta);

                transform.rotation = rotateVector;
            }
            // Otherwise, only update character rotation if NOT performing any action
            else if (!IsPerformingAction)
            {
                Vector3 rotationDir = (CameraController.currentLockonTarget.position - transform.position).normalized;
                rotationDir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(rotationDir);
                Quaternion rotateVector = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * delta);

                transform.rotation = rotateVector;
            }
        }
        else
        {
            // If performing any action other than rolling, don't allow character rotation
            if (IsPerformingAction && !IsRolling) return;

            Vector3 targetDir = ((_cameraGO.forward * _inputHandler.VerticalMove)
                + (_cameraGO.right * _inputHandler.HorizontalMove))
                .normalized;
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            Quaternion rotateVector = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * delta);

            transform.rotation = rotateVector;
        }
    }

    private void UpdateRollAndSprint(float delta)
    {
        if (_inputHandler.IsInteracting) return;

        if (_inputHandler.IsRolling)
        {
            _moveDirection = ((_cameraGO.forward * _inputHandler.VerticalMove)
                + (_cameraGO.right * _inputHandler.HorizontalMove));
            _moveDirection.y = 0;

            if (_inputHandler.FinalMovementAmount > 0)
            {
                _animator.UpdateMovementDirection(_moveDirection);
                transform.rotation = Quaternion.LookRotation(_moveDirection);
                _animator.PlayAnimation("DodgeRoll", true);
            }
            else
            {
                var backwards = (_cameraGO.forward * -1);
                backwards.y = 0;
                _animator.UpdateMovementDirection(backwards);
                transform.rotation = Quaternion.LookRotation(_cameraGO.position - transform.position);
                _animator.PlayAnimation("DodgeRoll", true);
            }

            _health.SetTemporaryInvuln(1f);
            IsRolling = true;
            _inputHandler.IsRolling = false;
        }
    }

    private void UpdateAttack(float delta)
    {
        if (_inputHandler.IsInteracting) return;

        if (_inputHandler.IsHeavyAttacking)
        {
            _weapon.HeavyAttack();
            _inputHandler.IsHeavyAttacking = false;
        }
        else if (_inputHandler.IsLightAttacking)
        {
            _weapon.LightAttack(_inputHandler.LightComboStep);
            _inputHandler.IsLightAttacking = false;
        }
        else if (_inputHandler.IsParrying)
        {
            _animator.PlayAnimation("Parry", true);
            _health.SetParryState(0.3f);
            _inputHandler.IsParrying = false;
        }
    }

    public void GetHit(bool isBlocking)
    {
        var onHitStateName = (isBlocking) ? "Hit_Block" : "Hit";

        _animator.PlayAnimation(onHitStateName, true);
        _weapon.DeactivateWeapon();
    }
}
