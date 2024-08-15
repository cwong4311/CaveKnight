using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterManager
{
    public Rigidbody RB;
    public CameraController CameraController;
    private GameObject _baseCam;

    [Header("Movement Parameters")]
    public float MovementSpeed = 5f;
    public float SprintSpeed = 8f;
    public float RotationSpeed = 10f;
    public bool IsSprinting;
    public bool IsRolling;
    public bool IsPerformingAction;
    public bool IsBlocking;
    public float BlockingMovementSpeed = 4f;

    [Header("Action Cost")]
    public float RollStaminaConsumption = 40;
    public float HeavyAttackStaminaConsumption = 30;
    public float ParryStaminaConsumption = 20;
    public float HealManaCost = 30;

    private Transform _cameraGO;
    private Vector3 _moveDirection;

    private PlayerInputHandler _inputHandler;
    private PlayerAnimationHandler _animator;
    private PlayerWeapon _weapon;
    private PlayerMagic _magic;
    private PlayerStatus _playerStatus;
    private PlayerSoundManager _playerSound;

    private bool isInHitStun = false;
    private Coroutine _hitStunCoroutine = null;

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

        _magic = GetComponentInChildren<PlayerMagic>();
        if (_magic != null)
        {
            _magic.Initialise();
        }

        _playerStatus = GetComponent<PlayerStatus>();
        _playerSound = GetComponentInChildren<PlayerSoundManager>();

        _cameraGO = Camera.main.transform;
        CameraController = FindObjectOfType<CameraController>();

        isInHitStun = false;
    }

    public void Update()
    {
        if (GameLogicManager.IsPaused) return;

        if (isInHitStun) return;

        float delta = Time.deltaTime;

        _inputHandler.IsInteracting = _animator.Anim.GetBool("IsInteracting");
        IsPerformingAction = !_animator.Anim.GetBool("CanMove");

        _inputHandler.ParseInput(delta);
        IsSprinting = _inputHandler.IsSprinting;

        // Don't allow running while blocking
        IsBlocking = _inputHandler.IsBlocking;
        if (IsBlocking) IsSprinting = false;
        if (_inputHandler.IsInteracting) IsBlocking = false;

        if (_inputHandler.IsInteracting == false && IsRolling)
        {
            IsRolling = false;
        }

        UpdateMovement(delta);
        UpdateRotation(delta);
        UpdateRollAndSprint(delta);
        UpdateAttack(delta);
        UpdateSpell(delta);
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
            // If performing an action, and the animator requests a tracking strength,
            // rotate player according to tracking strength
            if (IsPerformingAction && !IsRolling)
            {
                FaceLockedOnEnemy();
            }
            // While locked on, special rotation behaviour while running and rolling
            else if (IsSprinting || IsRolling)
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
                FaceLockedOnEnemy(delta);
            }
        }
        else
        {
            // If performing any action other than rolling, always face in camera direction
            Vector3 targetDir = Vector3.one;
            if (IsPerformingAction && !IsRolling)
            {
                // targetDir = _cameraGO.forward.normalized;
                return;
            }
            // Otherwise, when not performing any action OR rolling, face in the direction of the player
            else
            {
                targetDir = ((_cameraGO.forward * _inputHandler.VerticalMove)
                + (_cameraGO.right * _inputHandler.HorizontalMove))
                .normalized;
            }

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
            _inputHandler.IsRolling = false;
            if (_playerStatus.ConsumeStamina(RollStaminaConsumption) == false) return;

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

            _playerStatus.SetTemporaryInvuln(duration: 1f, keepCollision: false);
            IsRolling = true;
        }
    }

    private void UpdateAttack(float delta)
    {
        if (_inputHandler.IsInteracting) return;

        if (_inputHandler.IsHeavyAttacking)
        {
            _inputHandler.IsHeavyAttacking = false;
            if (_playerStatus.ConsumeStamina(HeavyAttackStaminaConsumption) == false) return;

            FaceLockedOnEnemy();
            _weapon.HeavyAttack();
        }
        else if (_inputHandler.IsLightAttacking)
        {
            _inputHandler.IsLightAttacking = false;

            FaceLockedOnEnemy();
            _weapon.LightAttack(_inputHandler.LightComboStep);
        }
        else if (_inputHandler.IsParrying)
        {
            _inputHandler.IsParrying = false;
            if (_playerStatus.ConsumeStamina(ParryStaminaConsumption) == false) return;

            FaceLockedOnEnemy();
            _animator.PlayAnimation("Parry", true);
            _playerStatus.SetParryState(0.3f);
        }
    }

    private void UpdateSpell(float delta)
    {
        if (_inputHandler.IsInteracting) return;

        if (_inputHandler.IsCastSpellOne)
        {
            _inputHandler.IsCastSpellOne = false;
            if (_playerStatus.ConsumeMana(HealManaCost) == false) return;

            _magic.CastHeal();
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

    public void TriggerParry(Collider collision)
    {
        if (_weapon == null) return;

        _weapon.ActivateParry(collision);
        _animator.ForceInteractable();
    }

    private IEnumerator HitStunTimer(float duration)
    {
        isInHitStun = true;
        _animator.Anim.speed = 0.01f;
        RB.isKinematic = true;

        yield return new WaitForSeconds(duration);

        isInHitStun = false;
        _animator.Anim.speed = 1f;
        RB.isKinematic = false;
    }

    public void GetHit(bool isBlocking)
    {
        var onHitStateName = (isBlocking) ? "Hit_Block" : "Hit";

        _animator.PlayAnimation(onHitStateName, true);
        _weapon.DeactivateWeapon();
    }

    public void Die()
    {
        _animator.PlayAnimation("Defeated", true);
        _weapon.DeactivateWeapon();

        GameLogicManager.OnGameOver?.Invoke();
    }

    public bool GetIsAttacking()
    {
        return _weapon.IsAttacking;
    }

    public void FaceLockedOnEnemy(float? delta = null)
    {
        if (CameraController.currentLockonTarget == null) return;

        Vector3 rotationDir = (CameraController.currentLockonTarget.position - transform.position).normalized;
        rotationDir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(rotationDir);
        Quaternion rotateVector = (delta != null) ? 
            Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * delta.Value) : targetRotation;

        transform.rotation = rotateVector;
    }
}
