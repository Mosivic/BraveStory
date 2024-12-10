using Godot;
using Miros.Core;

namespace BraveStory;

public partial class JumpAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_Jump;
    private int _jumpCount;


    protected override void ShareRes()
    {
        Res["JumpCount"] = _jumpCount;
    }

    protected override void Enter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Velocity.X, Agent.Attr("JumpVelocity"));
        _jumpCount++;
    }
}

