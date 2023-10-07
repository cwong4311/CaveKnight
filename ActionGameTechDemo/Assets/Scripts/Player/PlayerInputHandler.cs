using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public float HorizontalMove;
    public float VerticalMove;
    public float FinalMovementAmount;
    public float MouseX;
    public float MouseY;

    public bool IsRolling;
    public bool IsInteracting;

    private PlayerControls _inputActions;
    private CameraController _cameraController;

    private Vector2 _movementInput;
    private Vector2 _cameraInput;

    public void Awake()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerControls();
        }

        _cameraController = CameraController.instance;
    }

    public void Start()
    {
        _inputActions.Player.Movement.performed += OnPlayerMovement;
        _inputActions.Player.Camera.performed += OnCameraMovement;
        _inputActions.Player.Roll.started += OnRollStarted;
    }

    public void OnEnable()
    {
        _inputActions.Player.Movement.Enable();
        _inputActions.Player.Camera.Enable();
        _inputActions.Player.Roll.Enable();
    }

    public void OnDisable()
    {
        _inputActions.Disable();
    }

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;

        if (_cameraController != null)
        {
            _cameraController.FollowTarget(delta);
            _cameraController.HandleCameraRotation(delta, MouseX, MouseY);
        }
    }

    private void OnPlayerMovement(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void OnCameraMovement(InputAction.CallbackContext context)
    {
        _cameraInput = context.ReadValue<Vector2>();
    }

    public void ParseInput(float delta)
    {
        GetPlayerMovement(delta);
        GetCameraMovement(delta);
    }

    public void GetPlayerMovement(float delta)
    {
        if (IsInteracting) return;

        HorizontalMove = _movementInput.x;
        VerticalMove = _movementInput.y;
        FinalMovementAmount = Mathf.Clamp01(Mathf.Abs(HorizontalMove) + Mathf.Abs(VerticalMove));
    }

    public void GetCameraMovement(float delta)
    {
        MouseX = _cameraInput.x;
        MouseY = _cameraInput.y;
    }

    public void OnRollStarted(InputAction.CallbackContext context)
    {
        if (IsInteracting) return;

        IsRolling = true;
    }
}
