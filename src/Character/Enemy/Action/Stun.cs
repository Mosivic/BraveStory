using Miros.Core;

namespace BraveStory;

public class StunEnemyAction : Action<Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Action_Stun;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () => Context.StunTimer >= Context.StunDuration)
        ]
    );

    public override void Init(Enemy host, EnemyContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("idle");
        Context.StunTimer = 0.0f;
        Context.IsStunned = false;

        var stunEffect = new Effect
        {
            Tag = Tags.Effect_Buff,
            SourceAgent = OwnerAgent,
            RemovePolicy = RemovePolicy.WhenExited,
            DurationPolicy = DurationPolicy.Instant,
            Executions = [new CustomAttackDamageExecution(13)]
        };

        OwnerAgent.AddTaskFromState(ExecutorType.EffectExecutor, stunEffect); // 添加伤害效果
    }

    private void OnPhysicsUpdate(double delta)
    {
        Context.StunTimer += (float)delta;
    }
}