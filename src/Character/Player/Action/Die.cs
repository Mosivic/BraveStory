using Miros.Core;

namespace BraveStory;

public class DieAction : Stator<State, Player,PlayerShared>
{
    public override Tag StateTag  => Tags.State_Status_Die;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition AnyTransition  => new (Tags.State_Status_Die, () => Agent.Attr("HP") <= 0); 

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
