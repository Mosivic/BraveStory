using Godot;
using Miros.Core;

namespace BraveStory;

public partial class JumpAction : StateNode<Player>
{
    protected override Tag StateTag  => Tags.State_Action_Jump;
    protected override Tag LayerTag => Tags.StateLayer_Movement;
    protected override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    protected override Transition[] Transitions  => [
            new (Tags.State_Action_Fall),
        ];
    
    protected override void ShareRes()
    {
        Res["JumpCount"] = 0;
    }

    protected override void Enter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Velocity.X, Agent.Attr("JumpVelocity"));
        Res["JumpCount"]++;
    }
}

