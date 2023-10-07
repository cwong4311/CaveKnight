using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody RB;
    private GameObject _baseCam;

    public float MovementSpeed = 5f;
    public float SprintSpeed = 8f;
    public float RotationSpeed = 10f;
    public bool IsSprinting;

    private Transform _cameraGO;
    private Vector3 _moveDirection;

    private PlayerInputHandler _inputHandler;
    private PlayerAnimationHandler _animator;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _cameraGO = Camera.main.transform;

        _animator = GetComponentInChildren<PlayerAnimationHandler>();
        if (_animator != null)
        {
            _animator.Initialise();
        }
    }

    public void Update()
    {
        float delta = Time.deltaTime;

        _inputHandler.IsInteracting = _animator.Anim.GetBool("IsInteracting");
        _inputHandler.ParseInput(delta);
        IsSprinting = _inputHandler.IsSprinting;

        UpdateMovement(delta);
        UpdateRotation(delta);
        UpdateRollAndSprint(delta);
        UpdateAttack(delta);
    }

    private void UpdateMovement(float delta)
    {
        _moveDirection = ((_cameraGO.forward * _inputHandler.VerticalMove)
            + (_cameraGO.right * _inputHandler.HorizontalMove))
            .normalized;
        _moveDirection.y = 0f;
        
        float speed = (IsSprinting && _inputHandler.FinalMovementAmount > 0.5f) ? SprintSpeed : MovementSpeed;
        _moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, Vector3.zero);
        RB.velocity = projectedVelocity;

        _animator.UpdateAnimation(0f, _inputHandler.FinalMovementAmount, IsSprinting);
    }

    private void UpdateRotation(float delta)
    {
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
                transform.rotation = Quaternion.LookRotation(_moveDirection);
                _animator.PlayAnimation("DodgeRoll", true);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(_cameraGO.position - transform.position);
                _animator.PlayAnimation("DodgeRoll", true);
            }

            _inputHandler.IsRolling = false;
        }
    }

    private void UpdateAttack(float delta)
    {
        if (_inputHandler.IsInteracting) return;

        if (_inputHandler.IsHeavyAttacking)
        {
            _animator.PlayAnimation("HeavyAttack", true);
            _inputHandler.IsHeavyAttacking = false;
        }
        else if (_inputHandler.IsLightAttacking)
        {
            var attackStep = _inputHandler.LightComboStep;
            var attackName = attackStep switch
            {
                0 => "LightAttack1",
                1 => "LightAttack2",
                _ => "LightAttack1"
            };

            _animator.PlayAnimation(attackName, true);
            _inputHandler.IsLightAttacking = false;
        }
    }
}
