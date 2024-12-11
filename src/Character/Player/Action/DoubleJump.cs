using Godot;
using Miros.Core;

namespace BraveStory;

public partial class DoubleJumpAction : StateNode<State, Player,PlayerShared>
{
    public override Tag StateTag  => Tags.State_Action_DoubleJump;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition[] Transitions  => [
            new (Tags.State_Action_Fall),
        ];
    

    protected override void Enter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Velocity.X, Agent.Attr("JumpVelocity"));
        Shared.JumpCount = 0;
    }
}