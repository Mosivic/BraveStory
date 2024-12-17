using Godot;
using Miros.Core;

namespace BraveStory;

public class RunActionState : ActionState<Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Action_Run;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () => !Host.KeyDownMove()),
            new(Tags.State_Action_Jump, () => Host.KeyDownJump()),
            new(Tags.State_Action_Attack1, () => Host.KeyDownAttack()),
            new(Tags.State_Action_Sliding, () => Host.KeyDownSliding())
        ]
    );

    public override void Init(Player host, PlayerContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("run");
    }

    private void OnPhysicsUpdate(double delta)
    {
        Move(delta);
    }

    public void Move(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Host.Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * OwnerAgent.Atr("RunSpeed"), OwnerAgent.Atr("FloorAcceleration"));
        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        Host.Velocity = velocity;

        Host.UpdateFacing(direction);
        Host.MoveAndSlide();
    }
}