using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody RB;
    private GameObject _baseCam;

    public float MovementSpeed = 5;
    public float RotationSpeed = 10;

    private Transform _cameraGO;
    private PlayerInputHandler _inputHandler;
    private Vector3 _moveDirection;

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

        _inputHandler.ParseInput(delta);

        UpdateMovement(delta);
        UpdateRotation(delta);
        UpdateRollAndSprint(delta);
    }

    private void UpdateMovement(float delta)
    {
        _moveDirection = ((_cameraGO.forward * _inputHandler.VerticalMove)
            + (_cameraGO.right * _inputHandler.HorizontalMove))
            .normalized;
        _moveDirection.y = 0f;
        _moveDirection *= MovementSpeed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, Vector3.zero);
        RB.velocity = projectedVelocity;

        _animator.UpdateAnimation(0f, _inputHandler.FinalMovementAmount);
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
}
