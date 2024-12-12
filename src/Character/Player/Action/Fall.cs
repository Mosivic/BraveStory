using Godot;
using Miros.Core;

namespace BraveStory;

public class FallAction : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Fall;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () => Host.IsOnFloor()),
            new(Tags.State_Action_WallSlide, () => Host.IsHandColliding() && Host.IsFootColliding() && !Host.KeyDownMove()),
            new(Tags.State_Action_Jump, () => Host.KeyDownJump() && Context.JumpCount < Context.MaxJumpCount)
        ]
    );

    protected override void OnEnter()
    {
        Host.PlayAnimation("fall");
    }


    protected override void OnPhysicsUpdate(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Host.Velocity;

        // 确保空中移动控制合理
        velocity.X = Mathf.MoveToward(
            velocity.X,
            direction * Agent.Attr("RunSpeed"),
            Agent.Attr("AirAcceleration")
        );

        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Host.Velocity = velocity;

        Host.UpdateFacing(direction);
        Host.MoveAndSlide();
    }
}