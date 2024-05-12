namespace AI.Dragon
{
    public class LandingState : AI_State
    {
        private string _animationState = "AerialLand";

        public LandingState(EnemyController myController) : base(myController)
        {
        }

        public override void OnStateEnter(string fromAction)
        {
            base.OnStateEnter(fromAction);

            PlayAnimationState(_animationState);
        }

        public override void Update(float delta)
        {
            if (IsAnimationCompleted(_animationState))
            {
                MoveState("Idle");
            }
        }

        public override void OnStateExit(string toAction) { }
    }
}
