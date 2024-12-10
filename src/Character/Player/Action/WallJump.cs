using Godot;
using Miros.Core;

namespace BraveStory;

public partial class WallJumpAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Action_WallJump;

    protected override void Enter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Graphics.Scale.X * 400, -320);
        Res["JumpCount"] = 0;
    }
}
