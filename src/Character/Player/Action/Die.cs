using Miros.Core;

namespace BraveStory;

public class DieAction : Task<State, Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Status_Die;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Status_Die, () => Agent.Attr("HP") <= 0, TransitionMode.Normal, 0, true)
        ]
    );

    protected override void OnEnter()
    {
        Host.PlayAnimation("die");
        Host.ClearInteractions();
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        if (State.RunningTime > 1.0f) Host.QueueFree();
    }
}