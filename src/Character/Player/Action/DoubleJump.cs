using Godot;
using Miros.Core;

namespace BraveStory;

public partial class DoubleJumpAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_DoubleJump;

    protected override void Enter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Velocity.X, Agent.Attr("JumpVelocity"));
        Res["JumpCount"] = 0;
    }
}
