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
                "BasicAttack" => new AttackState(_enemyController),
                "Backstep" => new DodgeState(_enemyController),
                "Fireball" => new FireballState(_enemyController, false),
                "BackstepFireball" => new FireballState(_enemyController, true),
                "GroundedScream" => new FakeScreamState(_enemyController),
                "Hurt" => new HurtState(_enemyController),
                "Idle" => new IdleState(_enemyController),
                _ => new IdleState(_enemyController)
            };
        }
        else
        {
            return stateName switch
            {
                "Idle" => new IdleState(_enemyController),
                _ => new IdleState(_enemyController)
            };
        }
    }
}
