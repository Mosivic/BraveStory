using Miros.Core;

namespace BraveStory;

public class DieEnemyActionState : ActionState<EnemyContext>
{
    public override Tag Tag => Tags.State_Status_Die;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Status_Die, () => OwnerAgent.Atr("HP") <= 0, TransitionMode.Normal, 0, true)
    ];

    private Enemy _host;
    public override void Init(EnemyContext context)
    {
        base.Init(context);
        _host = context.Host;
        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("die");
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (_host.IsAnimationFinished()) _host.QueueFree();
    }
}