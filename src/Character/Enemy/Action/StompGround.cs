using Godot;
using Miros.Core;

namespace BraveStory;

public class StompGroundEnemyActionState : ActionState<Enemy, EnemyContext>
{
    public override Tag Tag => Tags.State_Action_StompGround;

    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => Context.StompTimer >= 1.0f),
    ];

    public override void Init(Enemy host, EnemyContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("idle"); // 播放跳跃动画
        Context.StompTimer = 0f;
        Context.IsStomping = true;

        Host.Velocity = new Vector2(0, -200f);
    }

    private void OnPhysicsUpdate(double delta)
    {
        Context.StompTimer += (float)delta;
        Host.MoveAndSlide();

        if (Context.StompTimer >= Context.StompDuration)
        {
            Host.PlayAnimation("StompBox");
            Host.Velocity = new Vector2(0, 800f); // 设置下砸力

            // FIXME: 
            // 1.HItAgent 的获取存在问题
            // 2.HitAgent 需要改为 HitAgent 列表
            if (Context.HitAgent != null)
            {
                var damageEffect = new Effect
                {
                    Tag = Tags.Effect_Buff,
                    SourceAgent = OwnerAgent,
                    RemovePolicy = RemovePolicy.WhenExited,
                    DurationPolicy = DurationPolicy.Instant,
                    Executions = [new DamageExecution()]
                };

                Context.HitAgent.AddEffect(damageEffect);

                Context.HitAgent.SwitchTaskFromTag(ExecutorType.MultiLayerExecutor, Tags.State_Action_Jump,
                    new MultiLayerSwitchTaskArgs(Tags.StateLayer_Movement));
                
                Context.HitAgent = null;
            }
        }
    }

    private void OnExit()
    {
        Context.IsStomping = false;
        Context.StompTimer = 0f;
    }
}