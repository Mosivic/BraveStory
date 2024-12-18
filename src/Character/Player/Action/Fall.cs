using Godot;
using Miros.Core;

namespace BraveStory;

public class FallActionState : ActionState
{
    private PlayerContext _ctx;
    private Player _host;

    public override Tag Tag => Tags.State_Action_Fall;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => _host.IsOnFloor()),
        new(Tags.State_Action_WallSlide, () => _host.IsHandColliding() && _host.IsFootColliding() && !_host.KeyDownMove()),
        new(Tags.State_Action_Jump, () => _host.KeyDownJump() && _ctx.JumpCount < _ctx.MaxJumpCount)
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
        _host.PlayAnimation("fall");
    }


    private void OnPhysicsUpdate(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = _host.Velocity;

        // 确保空中移动控制合理
        velocity.X = Mathf.MoveToward(
            velocity.X,
            direction * OwnerAgent.Atr("RunSpeed"),
            OwnerAgent.Atr("AirAcceleration")
        );

        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        _host.Velocity = velocity;

        _host.UpdateFacing(direction);
        _host.MoveAndSlide();
    }
}