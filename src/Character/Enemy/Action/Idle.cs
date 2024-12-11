using Miros.Core;

namespace BraveStory;

public partial class IdleEnemyAction : StateNode<State, Enemy>
{
    public override Tag StateTag => Tags.State_Action_Idle;

    public override Tag LayerTag => Tags.StateLayer_Movement;

    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    protected override void Enter()
    {
        Host.PlayAnimation("idle");
    }
}

