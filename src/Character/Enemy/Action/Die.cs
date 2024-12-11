using Miros.Core;

namespace BraveStory;

public partial class DieEnemyAction : Stator<State, Enemy,EnemyShared>
{
    public override Tag StateTag => Tags.State_Status_Die;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition AnyTransition => new (Tags.State_Action_Idle, () => Agent.Attr("HP") <= 0);
    
    
    protected override void Enter()
    {
        Host.PlayAnimation("die");
    }

    protected override void PhysicsUpdate(double delta)
    {
        if(Host.IsAnimationFinished()) Host.QueueFree();
    }
}