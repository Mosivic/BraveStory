using Godot;
using Miros.Core;

namespace BraveStory;

public class SlidingActionState : ActionState<PlayerContext>
{
    public override Tag Tag => Tags.State_Action_Sliding;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => _host.IsAnimationFinished())
    ];

    private Player _host;

    public override void Init(PlayerContext context)
    {
        base.Init(context);
        _host = context.Host;

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
        ExitFunc += OnExit;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("sliding_start");
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (_host.GetCurrentAnimation() == "sliding_start" && _host.IsAnimationFinished())
            _host.PlayAnimation("sliding_loop");
        else if (_host.GetCurrentAnimation() == "sliding_loop" && _host.IsAnimationFinished())
            _host.PlayAnimation("sliding_end");
        Slide(delta);
    }

    private void OnExit()
    {
        _host.SetHurtBoxMonitorable(true);
    }


    private void Slide(double delta)
    {
        var velocity = _host.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, 0, OwnerAgent.Atr("SlidingDeceleration") * (float)delta);
        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        _host.Velocity = velocity;
        _host.MoveAndSlide();
    }
}