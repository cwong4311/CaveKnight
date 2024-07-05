using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateFactory
{
    private EnemyController _enemyController;

    public AIStateFactory(EnemyController enemyController)
    {
        _enemyController = enemyController;
    }

    public AI_State GetAIStateByName(string enemyName, string stateName, float? healthPercentage = null)
    {
        return enemyName switch
        {
            "Dragon" => DragonResolver(stateName, healthPercentage),
            "Skeleton" => SkeletonResolver(stateName),
            "OrcAssassin" => OrcAssassinResolver(stateName),
            "Minotaur" => MinotaurResolver(stateName),
            _ => null
        };
    }

    public AI_State DragonResolver(string stateName, float? healthPercentage)
    {
        if (healthPercentage.HasValue == false || healthPercentage > 0.5)
        {
            return stateName switch
            {
                "BasicAttack" => new AI.Dragon.AttackState(_enemyController),
                "Backstep" => new AI.Dragon.DodgeState(_enemyController),
                "Fireball" => new AI.Dragon.FireballState(_enemyController, false),
                "BackstepFireball" => new AI.Dragon.FireballState(_enemyController, true),
                "GroundedScream" => new AI.Dragon.FakeScreamState(_enemyController),
                "Hurt" => new AI.Dragon.HurtState(_enemyController),
                "Idle" => new AI.Dragon.IdleState(_enemyController),
                "Die" => new AI.Dragon.DieState(_enemyController),
                _ => new AI.Dragon.IdleState(_enemyController)
            };
        }
        else
        {
            return stateName switch
            {
                "TakeOff" => new AI.Dragon.TakeOffState(_enemyController),
                "AerialIdle" => new AI.Dragon.AerialIdleState(_enemyController),
                "AerialFireball" => new AI.Dragon.AerialFireballState(_enemyController, true),
                "Landing" => new AI.Dragon.LandingState(_enemyController),
                "DiveBomb" => new AI.Dragon.DiveBombState(_enemyController),
                "TailSwipe" => new AI.Dragon.DoubleTailSwipe(_enemyController),
                "Charging" => new AI.Dragon.ChargingState(_enemyController),
                "TripleFireball" => new AI.Dragon.EnragedFireballState(_enemyController, false),
                "Hurt" => new AI.Dragon.HurtState(_enemyController),
                "Idle" => new AI.Dragon.EnrageIdleState(_enemyController),
                _ => DragonResolver(stateName, null)    // Refer to default state setup if can't find a match
            };;
        }
    }

    public AI_State SkeletonResolver(string stateName)
    {
        return stateName switch
        {
            "BasicAttack" => new AI.Skeleton.AttackState(_enemyController),
            "Idle" => new AI.Skeleton.IdleState(_enemyController),
            "Hurt" => new AI.Skeleton.HurtState(_enemyController),
            "Die" => new AI.Skeleton.DieState(_enemyController),
            _ => new AI.Skeleton.IdleState(_enemyController)
        };
    }

    public AI_State OrcAssassinResolver(string stateName)
    {
        return stateName switch
        {
            "Dodge" => new AI.OrcAssassin.DodgeState(_enemyController),
            "FuryAttack" => new AI.OrcAssassin.FuryAttackState(_enemyController),
            "Attack1" => new AI.OrcAssassin.PrimaryAttackState(_enemyController),
            "Attack2" => new AI.OrcAssassin.SecondaryAttackState(_enemyController),
            "Idle" => new AI.OrcAssassin.IdleState(_enemyController),
            "Hurt" => new AI.OrcAssassin.HurtState(_enemyController),
            "Die" => new AI.OrcAssassin.DieState(_enemyController),
            _ => new AI.OrcAssassin.IdleState(_enemyController)
        };
    }

    public AI_State MinotaurResolver(string stateName)
    {
        return stateName switch
        {
            "Attack1" => new AI.Minotaur.PrimaryAttackState(_enemyController),
            "Attack2" => new AI.Minotaur.SecondaryAttackState(_enemyController),
            "Kick" => new AI.Minotaur.KickState(_enemyController),
            "Idle" => new AI.Minotaur.IdleState(_enemyController),
            "Hurt" => new AI.Minotaur.HurtState(_enemyController),
            "Die" => new AI.Minotaur.DieState(_enemyController),
            _ => new AI.Minotaur.IdleState(_enemyController)
        };
    }
}
