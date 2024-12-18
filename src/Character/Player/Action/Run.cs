using Godot;
using Miros.Core;

namespace BraveStory;

public class RunActionState : ActionState
{
    private PlayerContext _ctx;
    private Player _host;

    public override Tag Tag => Tags.State_Action_Run;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => !_host.KeyDownMove()),
        new(Tags.State_Action_Jump, () => _host.KeyDownJump()),
        new(Tags.State_Action_Attack1, () => _host.KeyDownAttack()),
        new(Tags.State_Action_Sliding, () => _host.KeyDownSliding())
    ];

    public override void Init()
    {
        _ctx = Context as PlayerContext;
        _host = _ctx.Host;

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("run");
    }

    private void OnPhysicsUpdate(double delta)
    {
        Move(delta);
    }

    public void Move(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = _host.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * OwnerAgent.Atr("RunSpeed"), OwnerAgent.Atr("FloorAcceleration"));
        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        _host.Velocity = velocity;

        _host.UpdateFacing(direction);
        _host.MoveAndSlide();
    }
}