using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{
    public class AerialFireballState : FireballState
    {
        public AerialFireballState(EnemyController myController, bool isAerial) : base(myController, isAerial)
        {
            _aerialAnimationState = "Fly Fireball Shoot";
            _animationState = _aerialAnimationState;
            _delayBeforeShooting = 0.38f;

            _fireballFinishState = "AerialIdle";
        }

        public override void OnStateExit(string toAction) { }

        protected override void ShootFireball()
        {
            // Spawn 3 fireballs instead, one at player location
            // The other two to the left and right of player location
            // Don't use homing even for Aerial variant
            var playerLocation = _myController.TargetTransform.position + _myController.TargetTransform.up;
            ((DragonController)_myController).Fireball.SpawnFireball(playerLocation);
            ((DragonController)_myController).Fireball.SpawnFireball(playerLocation + _myController.transform.right * -5);
            ((DragonController)_myController).Fireball.SpawnFireball(playerLocation + _myController.transform.right * 5);
        }
    }
}
