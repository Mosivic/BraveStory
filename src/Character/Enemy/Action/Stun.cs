using Miros.Core;

namespace BraveStory;

public class StunEnemyActionState : ActionState
{
    private EnemyContext ctx;

    private Enemy host;
    public override Tag Tag => Tags.State_Action_Stun;

    public override Tag Layer => Tags.StateLayer_Movement;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Idle, () => ctx.StunTimer >= ctx.StunDuration)
    ];

    public override void Init()
    {
        ctx = Context as EnemyContext;
        host = ctx.Host;

        EnterFunc = OnEnter;
        PhysicsUpdateFunc = OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        host.PlayAnimation("idle");
        ctx.StunTimer = 0.0f;
        ctx.IsStunned = false;

        var stunEffect = new Effect
        {
            Tag = Tags.Effect_Buff,
            SourceAgent = OwnerAgent,
            RemovePolicy = RemovePolicy.WhenExited,
            DurationPolicy = DurationPolicy.Instant,
            Executions = [new CustomAttackDamageExecution(13)]
        };

        OwnerAgent.AddEffect(stunEffect); // 添加伤害效果
    }

    private void OnPhysicsUpdate(double delta)
    {
        ctx.StunTimer += (float)delta;
    }
}