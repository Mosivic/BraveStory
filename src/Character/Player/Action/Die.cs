using Miros.Core;

namespace BraveStory;

public class DieActionState : ActionState<Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Status_Die;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Status_Die, () => OwnerAgent.Atr("HP") <= 0, TransitionMode.Normal, 0, true)
        ]
    );

    public override void Init(Player host, PlayerContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("die");
        Host.ClearInteractions();
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (RunningTime > 1.0f) Host.QueueFree();
    }
}