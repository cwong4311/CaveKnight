using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{
    public class EnragedFireballState : FireballState
    {
        public EnragedFireballState(EnemyController myController, bool isAerial) : base(myController, isAerial)
        {
            _groundedAnimationState = "Fireball";
            _aerialAnimationState = "Fly Fireball Shoot";

            _animationState = isAerial ? _aerialAnimationState : _groundedAnimationState;
        }

        public override void OnStateExit(string toAction) { }

        protected override void ShootFireball()
        {
            // Spawn 3 fireballs instead, one at player location
            // The other two to the left and right of player location
            // Don't use homing even for Aerial variant
            var playerLocation = _myController.TargetTransform.position + _myController.TargetTransform.up;
            _myController.Fireball.SpawnFireball(playerLocation);
            _myController.Fireball.SpawnFireball(playerLocation + _myController.transform.right * -5);
            _myController.Fireball.SpawnFireball(playerLocation + _myController.transform.right * 5);
        }
    }
}
