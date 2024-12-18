using Miros.Core;

namespace BraveStory;

public class StunEnemyActionState : ActionState<Enemy, EnemyContext>
{
    public override Tag Tag => Tags.State_Action_Stun;

    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => Context.StunTimer >= Context.StunDuration)
    ];

    public override void Init(Enemy host, EnemyContext context)
    {
        base.Init(host, context);

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

        OwnerAgent.AddEffect(stunEffect); // 添加伤害效果
    }

    private void OnPhysicsUpdate(double delta)
    {
        Context.StunTimer += (float)delta;
    }
}