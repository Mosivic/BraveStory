using Miros.Core;

namespace BraveStory;

public class DieActionState : ActionState
{
    private PlayerContext _ctx;
    private Player _host;

    public override Tag Tag => Tags.State_Status_Die;
    public override Tag Layer => Tags.StateLayer_Movement;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Status_Die, () => OwnerAgent.Atr("HP") <= 0, TransitionMode.Normal, 0, true)
    ];

    public override void Init()
    {
        _ctx = Context as PlayerContext;
        _host = _ctx.Host;

        EnterFunc = OnEnter;
        PhysicsUpdateFunc = OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("die");
        _host.ClearInteractions();
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (RunningTime > 1.0f) _host.QueueFree();
    }
}