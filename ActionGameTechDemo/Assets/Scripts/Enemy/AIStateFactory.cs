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

    public AI_State GetAIStateByName(string stateName)
    {
        return stateName switch
        {
            "Bite" => new AttackState(_enemyController, true),
            "Tail" => new AttackState(_enemyController, false),
            "Backstep" => new DodgeState(_enemyController),
            "Fireball" => new FireballState(_enemyController, false),
            "BackstepFireball" => new FireballState(_enemyController, true),
            "Idle" => new IdleState(_enemyController),
            _ => new IdleState(_enemyController)
        };
    }
}
