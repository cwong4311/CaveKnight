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
            _ => null
        };
    }

    public AI_State DragonResolver(string stateName, float? healthPercentage)
    {
        if (healthPercentage.HasValue && healthPercentage > 0.5)
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
                _ => new AI.Dragon.IdleState(_enemyController)
            };
        }
        else
        {
            return stateName switch
            {
                "Idle" => new AI.Dragon.IdleState(_enemyController),
                _ => new AI.Dragon.IdleState(_enemyController)
            };
        }
    }
}
