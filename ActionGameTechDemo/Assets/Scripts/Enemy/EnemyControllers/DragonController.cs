using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DragonController : EnemyController
{
    [Header("Enemy-Specific Hitboxes")]
    public WeaponDamager Bite;
    public WeaponDamager Tail;
    public WeaponDamager Chest;
    public Fireball Fireball;

    public override void Awake()
    {
        base.Awake();

        Bite.SetWeaponTarget(targetEnemy: false);
        Tail.SetWeaponTarget(targetEnemy: false);
        Chest.SetWeaponTarget(targetEnemy: false);
    }
}
