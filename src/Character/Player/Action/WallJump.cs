using Godot;
using Miros.Core;

namespace BraveStory;

public partial class WallJumpAction : StateNode<State, Player,PlayerShared>
{
    public override Tag StateTag  => Tags.State_Action_WallJump;
    public override Tag LayerTag  => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    protected override void Enter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Graphics.Scale.X * 400, -320);
        
    }
}