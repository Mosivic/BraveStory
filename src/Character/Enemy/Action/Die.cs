using Miros.Core;

namespace BraveStory;

public class DieEnemyActionState : ActionState
{
    public override Tag Tag => Tags.State_Status_Die;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Status_Die, () => OwnerAgent.Atr("HP") <= 0, TransitionMode.Normal, 0, true)
    ];

    private Enemy host;
    private EnemyContext ctx;

    public override void Init()
    {
        ctx = Context as EnemyContext;
        host = ctx.Host;

        EnterFunc = OnEnter;
        PhysicsUpdateFunc = OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        host.PlayAnimation("die");
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (host.IsAnimationFinished()) host.QueueFree();
    }
}