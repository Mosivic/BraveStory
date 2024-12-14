using Miros.Core;

namespace BraveStory;

public class StunEnemyAction : Task<State, Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Stun;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () => Context.StunTimer >= Context.StunDuration)
        ]
    );


    protected override void OnEnter()
    {
        Host.PlayAnimation("idle");
        Context.StunTimer = 0.0f;
        Context.IsStunned = false;

        var stunEffect = new Effect
        {
            Tag = Tags.Effect_Buff,
            SourceAgent = Agent,
            RemovePolicy = RemovePolicy.WhenExited,
            DurationPolicy = DurationPolicy.Instant,
            Executions = [new CustomAttackDamageExecution(13)]
        };

        Agent.AddTaskFromState(ExecutorType.EffectExecutor, stunEffect); // 添加伤害效果
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        Context.StunTimer += (float)delta;
    }
}