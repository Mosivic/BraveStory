using Miros.Core;

namespace BraveStory;

public class DieEnemyActionState : ActionState<Enemy, EnemyContext>
{
    public override Tag Tag => Tags.State_Status_Die;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Status_Die, () => OwnerAgent.Atr("HP") <= 0, TransitionMode.Normal, 0, true)
    ];


    public override void Init(Enemy host, EnemyContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("die");
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (Host.IsAnimationFinished()) Host.QueueFree();
    }
}