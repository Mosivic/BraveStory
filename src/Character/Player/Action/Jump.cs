using Godot;
using Miros.Core;

namespace BraveStory;

public class JumpActionState : ActionState<PlayerContext>
{
    private Player _host;

    public override Tag Tag => Tags.State_Action_Jump;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Fall, () => RunningTime > 0.1f)
    ];


    public override void Init(PlayerContext context)
    {
        base.Init(context);
        _host = context.Host;

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("jump");
        _host.Velocity = new Vector2(_host.Velocity.X, OwnerAgent.Atr("JumpVelocity"));
        Context.JumpCount++;
    }

    private void OnPhysicsUpdate(double delta)
    {
        _host.MoveAndSlide();
    }
}