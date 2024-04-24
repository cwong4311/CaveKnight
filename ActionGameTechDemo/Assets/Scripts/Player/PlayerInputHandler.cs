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
    public bool IsSprinting;
    public bool IsInteracting;

    public bool IsLightAttacking;
    public bool IsHeavyAttacking;
    public bool IsBlocking;
    public bool IsParrying;
    public int LightComboStep = -1;

    public bool LockedOn;

    private PlayerControls _inputActions;
    private CameraController _cameraController;

    private Vector2 _movementInput;
    private Vector2 _cameraInput;

    private float _timeSinceLastRoll;
    private float _timeSinceLastAttackInput;
    private float _timeSinceLastAttackFinish;
    private float _timeSinceLastBlock;

    private bool _cursorLocked;

    public void Awake()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerControls();
        }

        _cameraController = FindObjectOfType<CameraController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _cursorLocked = true;
    }

    public void Start()
    {
        _inputActions.Player.Movement.performed += OnPlayerMovement;

        _inputActions.Player.Camera.performed += OnCameraMovement;

        _inputActions.Player.Roll.started += OnShiftDown;
        _inputActions.Player.Roll.canceled += OnShiftUp;

        _inputActions.Player.Attack.started += OnAttackButtonDown;
        _inputActions.Player.Attack.canceled += OnAttackButtonUp;

        _inputActions.Player.Block.started += OnBlockButtonDown;
        _inputActions.Player.Block.canceled += OnBlockButtonUp;

        _inputActions.Player.Lockon.performed += OnLockon;

        _inputActions.Player.ToggleCursor.performed += OnEscapeToggle;
    }

    public void OnEnable()
    {
        _inputActions.Player.Movement.Enable();
        _inputActions.Player.Camera.Enable();
        _inputActions.Player.Roll.Enable();
        _inputActions.Player.Attack.Enable();
        _inputActions.Player.Block.Enable();
        _inputActions.Player.Lockon.Enable();
        _inputActions.Player.ToggleCursor.Enable();
    }

    public void OnDisable()
    {
        _inputActions.Disable();
    }

    public void Update()
    {
        if (IsInteracting)
        {
            _timeSinceLastAttackInput = 0f;
            IsRolling = false;
            IsLightAttacking = false;
            IsHeavyAttacking = false;
            IsParrying = false;
        }
        else
        {
            if (_timeSinceLastAttackInput > 0.05f)
            {
                var buttonReleaseDelay = Time.time - _timeSinceLastAttackInput;
                if (buttonReleaseDelay > 0.4f)
                {
                    IsHeavyAttacking = true;
                    LightComboStep = -1;

                    _timeSinceLastAttackInput = 0f;
                    _timeSinceLastAttackFinish = Time.time;
                }
            }
        }
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

    public void OnShiftDown(InputAction.CallbackContext context)
    {
        if (IsInteracting)
        {
            IsSprinting = true;
            _timeSinceLastRoll = 0f;
        }
        else
        {
            IsSprinting = true;
            IsRolling = false;
            _timeSinceLastRoll = Time.time;
        }
    }

    public void OnShiftUp(InputAction.CallbackContext context)
    {
        if (Time.time - _timeSinceLastRoll < 0.3f && _timeSinceLastRoll > 0.01f)
        {
            IsRolling = true;
        }

        IsSprinting = false;
        _timeSinceLastRoll = 0f;
    }

    public void OnAttackButtonDown(InputAction.CallbackContext context)
    {
        if (IsInteracting) return;

        _timeSinceLastAttackInput = Time.time;
    }

    public void OnAttackButtonUp(InputAction.CallbackContext context)
    {
        if (IsInteracting) return;

        if (Time.time - _timeSinceLastAttackFinish > 1.5f)
        {
            LightComboStep = -1;
        }

        if (_timeSinceLastAttackInput > 0.05f)
        {
            var buttonReleaseDelay = Time.time - _timeSinceLastAttackInput;
            if (buttonReleaseDelay > 0.35f)
            {
                IsHeavyAttacking = true;
                LightComboStep = -1;
            }
            else if (buttonReleaseDelay <= 0.35f)
            {
                LightComboStep = (LightComboStep + 1) % 2;
                IsLightAttacking = true;
            }
        }

        _timeSinceLastAttackInput = 0f;
        _timeSinceLastAttackFinish = Time.time;
    }

    public void OnBlockButtonDown(InputAction.CallbackContext context)
    {
    }

    public void OnBlockButtonUp(InputAction.CallbackContext context)
    {
    }

    public void OnLockon(InputAction.CallbackContext context)
    {
        if (!LockedOn)
        {
            LockedOn = _cameraController.HandleLockon();
        }
        else
        {
            var canCycle = _cameraController.CycleLockon();
            if (canCycle == false)
            {
                _cameraController.ClearLockon();
                LockedOn = false;
            }
        }
    }
    public void OnEscapeToggle(InputAction.CallbackContext context)
    {
        if (_cursorLocked)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _cursorLocked = false;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _cursorLocked = true;
        }
    }
}
