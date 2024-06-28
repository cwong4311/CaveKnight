using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : EnemyController
{
    [Header("Enemy-Specific Hitboxes")]
    public WeaponDamager Bite;
    public WeaponDamager Tail;
    public WeaponDamager Chest;
    public Fireball Fireball;

    [Header("Boss HUD Name")]
    public string HUDBossName;

    protected int _bossBGMState = -1;

    public override void Awake()
    {
        base.Awake();

        Bite.SetWeaponTarget(targetEnemy: false);
        Tail.SetWeaponTarget(targetEnemy: false);
        Chest.SetWeaponTarget(targetEnemy: false);
    }

    protected override void LateUpdate()
    {
        var healthHUD = _enemyHealth.GetHealthHUD();

        if (TargetTransform != null)
        {
            if (_enemyHealth.HealthPercentage > 0.5)
            {
                if (_bossBGMState == 0) return;

                _bossBGMState = 0;
                BGMMusicManager.PlayBossBGM(_bossBGMState);

                healthHUD?.ShowHealthBar();
                healthHUD?.SetBossName(HUDBossName);
            }
            else
            {
                if (_bossBGMState == 1) return;

                _bossBGMState = 1;
                BGMMusicManager.PlayBossBGM(_bossBGMState);

                healthHUD?.ShowHealthBar();
                healthHUD?.SetBossName(HUDBossName);
            }

            return;
        }

        _bossBGMState = -1;
        healthHUD?.HideHealthBar();
    }
}
