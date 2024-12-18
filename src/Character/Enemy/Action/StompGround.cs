using Godot;
using Miros.Core;

namespace BraveStory;

public class StompGroundEnemyActionState : ActionState<EnemyContext>
{
    public override Tag Tag => Tags.State_Action_StompGround;

    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => Context.StompTimer >= 1.0f),
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
        _host.PlayAnimation("idle"); // 播放跳跃动画
        Context.StompTimer = 0f;
        Context.IsStomping = true;

        _host.Velocity = new Vector2(0, -200f);
    }

    private void OnPhysicsUpdate(double delta)
    {
        Context.StompTimer += (float)delta;
        _host.MoveAndSlide();

        if (Context.StompTimer >= Context.StompDuration)
        {
            _host.PlayAnimation("StompBox");
            _host.Velocity = new Vector2(0, 800f); // 设置下砸力

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
                    new MultiLayerSwitchArgs(Tags.StateLayer_Movement));
                
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