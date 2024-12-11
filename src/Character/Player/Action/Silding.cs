using Godot;
using Miros.Core;

namespace BraveStory;

public partial class SlidingAction : StateNode<Player>
{
    protected override Tag StateTag  => Tags.State_Action_Sliding;
    protected override Tag LayerTag  => Tags.StateLayer_Movement;
    protected override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    protected override Transition[] Transitions  => [
            new (Tags.State_Action_Idle, () => Host.IsAnimationFinished()),
        ];

    protected override void Enter()
    {
        Host.PlayAnimation("sliding_start");
    }

    protected override void PhysicsUpdate(double delta)
    {
        if (Host.GetCurrentAnimation() == "sliding_start" && Host.IsAnimationFinished())
            Host.PlayAnimation("sliding_loop");
        else if (Host.GetCurrentAnimation() == "sliding_loop" && Host.IsAnimationFinished())
            Host.PlayAnimation("sliding_end");
        Slide(delta);
    }

    protected override void Exit()
    {
        Host.SetHurtBoxMonitorable(true);
    }


    private void Slide(double delta)
    {
        var velocity = Host.Velocity;
		velocity.X = Mathf.MoveToward(velocity.X, 0, Agent.Attr("SlidingDeceleration") * (float)delta);
		velocity.Y += (float)delta * Agent.Attr("Gravity");
		Host.Velocity = velocity;
		Host.MoveAndSlide();
	}
}