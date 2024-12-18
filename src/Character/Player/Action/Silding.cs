using Godot;
using Miros.Core;

namespace BraveStory;

public class SlidingActionState : ActionState<Player, PlayerContext>
{
    public override Tag Tag => Tags.State_Action_Sliding;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => Host.IsAnimationFinished())
    ];

    public override void Init(Player host, PlayerContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
        ExitFunc += OnExit;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("sliding_start");
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (Host.GetCurrentAnimation() == "sliding_start" && Host.IsAnimationFinished())
            Host.PlayAnimation("sliding_loop");
        else if (Host.GetCurrentAnimation() == "sliding_loop" && Host.IsAnimationFinished())
            Host.PlayAnimation("sliding_end");
        Slide(delta);
    }

    private void OnExit()
    {
        Host.SetHurtBoxMonitorable(true);
    }


    private void Slide(double delta)
    {
        var velocity = Host.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, 0, OwnerAgent.Atr("SlidingDeceleration") * (float)delta);
        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        Host.Velocity = velocity;
        Host.MoveAndSlide();
    }
}