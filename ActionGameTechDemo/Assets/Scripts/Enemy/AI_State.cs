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
            "Bite" => new AttackState(_enemyController),
            "Tail" => new AttackState(_enemyController),
            "Dodge" => new DodgeState(_enemyController),
            "Idle" => new IdleState(_enemyController),
            _ => new IdleState(_enemyController)
        };
    }
}
