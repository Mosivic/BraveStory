using Godot;
using Miros.Core;

namespace BraveStory;

public class JumpAction : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Jump;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Fall, () => State.RunningTime > 0.1f)
        ]
    );

    protected override void OnEnter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Velocity.X, Agent.Atr("JumpVelocity"));
        Context.JumpCount++;
    }

    public override void PhysicsUpdate(double delta)
    {
        Host.MoveAndSlide();
    }
}