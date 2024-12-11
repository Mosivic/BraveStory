using Miros.Core;

namespace BraveStory;

public partial class DieEnemyAction : StateNode<State, Enemy>
{
    public override Tag StateTag => Tags.State_Status_Die;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    protected override void Enter()
    {
        Host.PlayAnimation("die");
    }

    protected override void PhysicsUpdate(double delta)
    {
        if(Host.IsAnimationFinished()) Host.QueueFree();
    }
}