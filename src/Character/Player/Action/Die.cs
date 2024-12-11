using Miros.Core;

namespace BraveStory;

public partial class DieAction : StateNode<Player>
{
    protected override Tag StateTag  => Tags.State_Status_Die;
    protected override Tag LayerTag => Tags.StateLayer_Movement;
    protected override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    protected override Transition AnyTransition  => new (Tags.State_Status_Die, () => Agent.Attr("HP") <= 0); 

    protected override void Enter()
    {
        Host.PlayAnimation("die");
        Host.ClearInteractions();
    }

    protected override void PhysicsUpdate(double delta)
    {
        if (Host.IsAnimationFinished())
        {
            Host.QueueFree();
        }
    }
}
