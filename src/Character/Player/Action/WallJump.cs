using Godot;
using Miros.Core;

namespace BraveStory;

public class WallJumpAction : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_WallJump;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Fall)
        ]
    );


    protected override void OnEnter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Graphics.Scale.X * 400, -320);
    }
}