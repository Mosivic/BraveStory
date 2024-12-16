using Miros.Core;

namespace BraveStory;

public class DieEnemyAction : Task<State, Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Status_Die;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Status_Die, () => Agent.Atr("HP") <= 0, TransitionMode.Normal, 0, true)
        ]
    );


    protected override void OnEnter()
    {
        Host.PlayAnimation("die");
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        if (Host.IsAnimationFinished()) Host.QueueFree();
    }
}