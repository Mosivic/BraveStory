using Godot;
using Miros.Core;

namespace BraveStory;

public class WallJumpAction : Task<State, Player,PlayerContext>
{
    public override Tag StateTag  => Tags.State_Action_WallJump;
    public override Tag LayerTag  => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    protected override void OnEnter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Graphics.Scale.X * 400, -320);
    }
}