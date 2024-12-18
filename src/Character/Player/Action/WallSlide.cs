using Godot;
using Miros.Core;

namespace BraveStory;

public class WallSlideActionState : ActionState
{
    private PlayerContext _ctx;
    private Player _host;
    public override Tag Tag => Tags.State_Action_WallSlide;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => _host.IsOnFloor()),
        new(Tags.State_Action_Fall, () => !_host.IsFootColliding()),
        new(Tags.State_Action_WallJump, () => _host.KeyDownJump())
    ];

    public override void Init()
    {
        _ctx = Context as PlayerContext;
        _host = _ctx.Host;

        EnterFunc = OnEnter;
        PhysicsUpdateFunc = OnPhysicsUpdate;
    }


    private void OnEnter()
    {
        _host.PlayAnimation("wall_sliding");
    }

    private void OnPhysicsUpdate(double delta)
    {
        var velocity = _host.Velocity;
        velocity.Y = Mathf.Min(velocity.Y + (float)delta * OwnerAgent.Atr("Gravity"), 600);
        _host.Velocity = velocity;

        _host.UpdateFacing(0);
        _host.MoveAndSlide();
    }
}