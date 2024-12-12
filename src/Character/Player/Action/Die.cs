using Miros.Core;

namespace BraveStory;

public class DieAction : Task<State, Player,PlayerContext>
{
    public override Tag StateTag  => Tags.State_Status_Die;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;
    public override Transition[] Transitions  => [
            new (Tags.State_Status_Die, () => Agent.Attr("HP") <= 0,TransitionMode.Force,0,true)
        ]; 

    protected override void OnEnter()
    {
        Host.PlayAnimation("die");
        Host.ClearInteractions();
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        if (Host.IsAnimationFinished())
        {
            Host.QueueFree();
        }
    }
}
