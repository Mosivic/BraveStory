using Miros.Core;

namespace BraveStory;

public class DieEnemyAction : Task<State, Enemy, EnemyContext>
{
    public override Tag StateTag => Tags.State_Status_Die;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Status_Die, () => Agent.Attr("HP") <= 0, TransitionMode.Normal, 0, true)
    ];


    protected override void OnEnter()
    {
        Host.PlayAnimation("die");
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        if (Host.IsAnimationFinished()) Host.QueueFree();
    }
}