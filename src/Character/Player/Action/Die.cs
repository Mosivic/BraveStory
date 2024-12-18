using Miros.Core;

namespace BraveStory;

public class DieActionState : ActionState<Player, PlayerContext>
{
    public override Tag Tag => Tags.State_Status_Die;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Status_Die, () => OwnerAgent.Atr("HP") <= 0, TransitionMode.Normal, 0, true)
    ];

    public override void Init(Player host, PlayerContext context)
    {
        base.Init(host, context);

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