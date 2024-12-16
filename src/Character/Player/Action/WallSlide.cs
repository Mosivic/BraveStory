using Godot;
using Miros.Core;

namespace BraveStory;

public class WallSlideAction : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_WallSlide;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () => Host.IsOnFloor()),
            new(Tags.State_Action_Fall, () => !Host.IsFootColliding()),
            new(Tags.State_Action_WallJump, () => Host.KeyDownJump())
        ]
    );


    protected override void OnEnter()
    {
        Host.PlayAnimation("wall_sliding");
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        var velocity = Host.Velocity;
        velocity.Y = Mathf.Min(velocity.Y + (float)delta * Agent.Atr("Gravity"), 600);
        Host.Velocity = velocity;

        Host.UpdateFacing(0);
        Host.MoveAndSlide();
    }
}