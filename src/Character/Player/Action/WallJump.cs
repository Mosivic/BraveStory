using Godot;
using Miros.Core;

namespace BraveStory;

public partial class WallJumpAction : StateNode<Player>
{
    protected override Tag StateTag  => Tags.State_Action_WallJump;
    protected override Tag LayerTag  => Tags.StateLayer_Movement;
    protected override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    protected override void Enter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Graphics.Scale.X * 400, -320);
        Res["JumpCount"] = 0;
    }
}
