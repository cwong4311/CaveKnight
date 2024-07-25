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

    public bool IsCastSpellOne;

    public bool LockedOn;

    [SerializeField]
    private PlayerInputActionBind _inputActions;
    private CameraController _cameraController;

    private Vector2 _movementInput;
    private Vector2 _cameraInput;

    private float _timeSinceLastRoll;
    private float _timeSinceLastAttackInput;
    private float _timeSinceLastAttackFinish;
    private float _timeSinceLastBlock;

    public void Awake()
    {
        if (_inputActions == null)
        {
            throw new MissingComponentException("PlayerInputHandler is missing a PlayerInputActionBind mapping reference");
        }

        _cameraController = FindObjectOfType<CameraController>();
    }

    public void Start()
    {
        _inputActions.Movement.action.performed += OnPlayerMovement;

        _inputActions.Camera.action.performed += OnCameraMovement;

        _inputActions.Roll.action.started += OnShiftDown;
        _inputActions.Roll.action.canceled += OnShiftUp;

        _inputActions.Attack.action.started += OnAttackButtonDown;
        _inputActions.Attack.action.canceled += OnAttackButtonUp;

        _inputActions.Block.action.started += OnBlockButtonDown;
        _inputActions.Block.action.canceled += OnBlockButtonUp;

        _inputActions.Lockon.action.performed += OnLockon;
        _inputActions.SwapTarget.action.performed += OnChangeLockonTarget;

        _inputActions.Heal.action.performed += OnCastSpellOne;

        _inputActions.Pause.action.performed += OnEscapeToggle;
    }

    public void OnDestroy()
    {
        _inputActions.Movement.action.performed -= OnPlayerMovement;

        _inputActions.Camera.action.performed -= OnCameraMovement;

        _inputActions.Roll.action.started -= OnShiftDown;
        _inputActions.Roll.action.canceled -= OnShiftUp;

        _inputActions.Attack.action.started -= OnAttackButtonDown;
        _inputActions.Attack.action.canceled -= OnAttackButtonUp;

        _inputActions.Block.action.started -= OnBlockButtonDown;
        _inputActions.Block.action.canceled -= OnBlockButtonUp;

        _inputActions.Lockon.action.performed -= OnLockon;
        _inputActions.SwapTarget.action.performed -= OnChangeLockonTarget;

        _inputActions.Heal.action.performed -= OnCastSpellOne;

        _inputActions.Pause.action.performed -= OnEscapeToggle;
    }

    public void OnEnable()
    {
        _inputActions.Movement.action.Enable();
        _inputActions.Camera.action.Enable();
        _inputActions.Roll.action.Enable();
        _inputActions.Attack.action.Enable();
        _inputActions.Block.action.Enable();
        _inputActions.Lockon.action.Enable();
        _inputActions.SwapTarget.action.Enable();
        _inputActions.Heal.action.Enable();
        _inputActions.Pause.action.Enable();
    }

    public void OnDisable()
    {
        _inputActions.Movement.action.Disable();
        _inputActions.Camera.action.Disable();
        _inputActions.Roll.action.Disable();
        _inputActions.Attack.action.Disable();
        _inputActions.Block.action.Disable();
        _inputActions.Lockon.action.Disable();
        _inputActions.SwapTarget.action.Disable();
        _inputActions.Heal.action.Disable();
        _inputActions.Pause.action.Disable();
    }

    public void Update()
    {
        if (_cameraController != null)
        {
            _cameraController.FollowTarget(Time.deltaTime);
        }

        if (GameLogicManager.IsPaused) return;

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
                var buttonHeldTime = Time.time - _timeSinceLastAttackInput;
                if (buttonHeldTime > 0.2f)
                {
                    IsHeavyAttacking = true;
                    LightComboStep = -1;

                    _timeSinceLastAttackInput = 0f;
                    _timeSinceLastAttackFinish = Time.time;
                }
            }

            if (_timeSinceLastBlock > 0.03f)
            {
                var buttonHeldTime = Time.time - _timeSinceLastBlock;
                if (buttonHeldTime > 0.05f)
                {
                    IsBlocking = true;
                    IsParrying = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;

        if (_cameraController != null)
        {
            // If camera is currently locked on, but there's no actual target locked on
            // Likely target died. Try to swap targets, or stop lockon if no targets
            if (_cameraController.currentLockonTarget == null && LockedOn)
            {
                var canCycle = _cameraController.CycleLockon();
                if (canCycle == false)
                {
                    _cameraController.ClearLockon(forceSmoothUnlock : true);
                    LockedOn = false;
                }
            }

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
        if (Time.time - _timeSinceLastRoll < 0.2f && _timeSinceLastRoll > 0.01f)
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
                LightComboStep = (LightComboStep + 1) % 3;
                IsLightAttacking = true;
            }
        }

        _timeSinceLastAttackInput = 0f;
        _timeSinceLastAttackFinish = Time.time;
    }

    public void OnBlockButtonDown(InputAction.CallbackContext context)
    {
        if (IsInteracting) return;

        _timeSinceLastBlock = Time.time;
    }

    public void OnBlockButtonUp(InputAction.CallbackContext context)
    {
        if (_timeSinceLastBlock > 0.03f)
        {
            var buttonReleaseDelay = Time.time - _timeSinceLastBlock;
            if (buttonReleaseDelay <= 0.2f)
            {
                IsBlocking = false;
                IsParrying = true;
            }
            else
            {
                IsBlocking = false;
                IsParrying = false;
            }
        }

        _timeSinceLastBlock = 0f;
    }

    public void OnLockon(InputAction.CallbackContext context)
    {
        if (_cameraController.IsLockOnReady == false) return;

        if (!LockedOn)
        {
            LockedOn = _cameraController.HandleLockon();
        }
        else
        {
            _cameraController.ClearLockon();
            LockedOn = false;
        }
    }

    public void OnChangeLockonTarget(InputAction.CallbackContext context)
    {
        if (_cameraController.IsLockOnReady == false) return;

        if (LockedOn)
        {
            _cameraController.CycleLockon();
        }
    }

    public void OnEscapeToggle(InputAction.CallbackContext context)
    {
        GameLogicManager.OnPause?.Invoke();
    }

    public void OnCastSpellOne(InputAction.CallbackContext context)
    {
        if (IsInteracting) return;

        IsCastSpellOne = true;
    }
}
