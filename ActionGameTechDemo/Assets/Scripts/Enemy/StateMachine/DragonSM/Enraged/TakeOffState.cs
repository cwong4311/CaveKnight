namespace AI.Dragon
{
    public class TakeOffState : AI_State
    {
        private string _animationState = "Take Off";

        public TakeOffState(EnemyController myController) : base(myController)
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
                MoveState("AerialIdle");
            }
        }

        public override void OnStateExit(string toAction) { }
    }
}
