using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float LightDamage1;
    public float LightDamage2;
    public float LightDamage3;
    public float HeavyDamage;

    public WeaponDamager _weaponHolder;
    private PlayerAnimationHandler _animatorHandler;

    private string _currentAttackName;
    private float _currentAttackDamage;

    public bool IsAttacking;

    public void Initialise()
    {
        _animatorHandler = GetComponent<PlayerAnimationHandler>();

        if (_weaponHolder != null)
        {
            _weaponHolder.SetWeaponTarget(true);
        }
    }

    public void LightAttack(int attackStep = 0)
    {
        _currentAttackName = "";
        _currentAttackDamage = 0f;

        switch (attackStep)
        {
            default:
            case 0:
                _currentAttackName = "LightAttack1";
                _currentAttackDamage = LightDamage1;
                break;
            case 1:
                _currentAttackName = "LightAttack2";
                _currentAttackDamage = LightDamage2;
                break;
            case 2:
                _currentAttackName = "LightAttack3";
                _currentAttackDamage = LightDamage3;
                break;
        }

        _animatorHandler?.PlayAnimation(_currentAttackName, true);        
    }

    public void HeavyAttack()
    {
        _currentAttackDamage = HeavyDamage;
        _animatorHandler?.PlayAnimation("HeavyAttack", true);
    }

    /// <summary>
    /// Let weapon animation EVENT callback handle this call
    /// </summary>
    public void ActivateWeapon()
    {
        IsAttacking = true;
        _weaponHolder.ActivateWeapon(_currentAttackDamage);
    }

    /// <summary>
    /// Let weapon animation EVENT callback handle this call
    /// </summary>
    public void DeactivateWeapon()
    {
        IsAttacking = false;
        _weaponHolder.DeactivateWeapon();
    }
}
