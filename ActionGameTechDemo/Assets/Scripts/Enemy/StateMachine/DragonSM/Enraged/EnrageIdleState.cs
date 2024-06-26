using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Dragon
{
    public class EnrageIdleState : AI_State
    {
        public float MinIdleTime = 0.3f;
        public float MaxIdleTime = 0.6f;

        public bool IsInIdle;
        private float _idleTime;

        public EnrageIdleState(EnemyController myController) : base(myController)
        {
            _stateType = AIStateType.Idle;
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            // if transitioning phases, don't have any idle time
            if (fromAction == "Idle")
            {
                MinIdleTime = 0.05f; MaxIdleTime = 0.1f;
            }
            // extend delay after a special action
            else if (fromAction == "FirePillar")
            {
                MinIdleTime = 0.4f; MaxIdleTime = 0.7f;
            }
            else if (fromAction == "AerialLand" || fromAction == "DiveBomb")
            {
                MinIdleTime = 1f; MaxIdleTime = 1.3f;
            }

            _idleTime = UnityEngine.Random.Range(MinIdleTime, MaxIdleTime);
            _myController.UpdateMovementParameters(0f, 0f, false);

            IsInIdle = false;
            // All animations will naturally return to Idle anim state.
            // Wait for it to do so
        }

        public override void Update(float delta, bool isInHitStun)
        {
            base.Update(delta, isInHitStun);

            // Wait until animation is back in idle
            IsInIdle = _animator.GetBool("Idle");
            if (IsInIdle == false) return;

            if (_stateActive == false) return;

            // Always enable gravity when back in Idle state
            _myController.ToggleGravity(true);

            if (Time.time - _timeAtStateEnter >= _idleTime)
            {
                _myController.RestoreEnemyScale();
                
                var distance = CheckEnemyDistance();
                if (distance.HasValue)
                {
                    PerformAction(distance.Value);
                }
            }
        }

        public override void OnStateExit(string toAction) { }

        private float? CheckEnemyDistance()
        {
            if (_myController.TargetTransform == null)
            {
                var colliders = Physics.OverlapSphere(_animator.transform.position, _myController.PlayerDetectionRange);
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        _myController.TargetTransform = collider.transform;
                    }
                }
            }
            else
            {
                var distance = Vector3.Distance(_transform.position, _myController.TargetTransform.position);
                if (distance > _myController.MaxDistance)
                {
                    _myController.TargetTransform = null;
                }
                else
                {
                    return distance;
                }
            }

            return null;
        }

        private void PerformAction(float distance)
        {
            // At any distance, 25% take off
            if (UnityEngine.Random.Range(0, 4) == 0)
            {
                var aerial = CheckTakeoff();
                if (aerial) return;
            }

            // Otherwise, if still grounded
            // Perform actions based on distance, and with a chance
            int randomRes = UnityEngine.Random.Range(0, 3);

            // from range 0f ~ 5f: Melee
            if (distance <= 10)
            {
                if (randomRes > 0)
                {
                    if (CheckTailSwipe()) return;
                }
            }
            // from range 5f ~ 25f: Mid
            else if (distance < _myController.MaxDistance / 2)
            {
                if (randomRes == 0)
                {
                    if (CheckFireball()) return;
                }
                else if (randomRes == 1)
                {
                    if (CheckBackstep()) return;
                }
            }
            // from range 25f ~ 50f: Far
            else
            {
                if (randomRes == 0)
                {
                    if (CheckFireball()) return;
                }
                else if (randomRes == 1)
                {
                    if (CheckBackstep()) return;
                }
            }

            // If the general conditions fail, try to Charge
            // If even that fails, then go through each action regardless of range
            // and use the first one that returns Doable.
            var canCharge = CheckCharging();
            if (!canCharge)
            {
                var alternativeChecklist = new List<Action>() {
                        () => CheckFireball(),
                        () => CheckBackstep(),
                        () => CheckTakeoff()
                        // Don't include tail swipe, as it only works in close range
                    };

                foreach (var altAction in alternativeChecklist)
                {
                    altAction.Invoke();
                    if (!_stateActive) return;
                }
            }
        }

        private bool CheckBackstep()
        {
            if (_myController.TargetTransform == null) return false;

            if (GetTimesRecentlyExecuted("Backstep") < 2)
            {
                MoveState("Backstep");
                return true;
            }

            return false;
        }

        private bool CheckTakeoff()
        {
            if (_myController.TargetTransform == null) return false;

            int recentAirborne = 0;
            recentAirborne += GetTimesRecentlyExecuted("DiveBomb");
            recentAirborne += GetTimesRecentlyExecuted("AerialLand");

            // Only allow taking off once every (at least) 5 actions
            if (recentAirborne < 1)
            {
                MoveState("TakeOff");
                return true;
            }

            return false;
        }

        private bool CheckFireball()
        {
            if (_myController.TargetTransform == null) return false;

            if (GetTimesRecentlyExecuted("TripleFireball") < 3)
            {
                MoveState("TripleFireball");
                return true;
            }

            return false;
        }

        private bool CheckTailSwipe()
        {
            if (_myController.TargetTransform == null) return false;

            if (GetTimesRecentlyExecuted("TailSwipe") < 2)
            {
                MoveState("TailSwipe");
                return true;
            }

            return false;
        }

        private bool CheckCharging()
        {
            if (_myController.TargetTransform == null) return false;

            if (GetTimesRecentlyExecuted("Charging") < 3)
            {
                MoveState("Charging");
                return true;
            }

            return false;
        }
    }
}
