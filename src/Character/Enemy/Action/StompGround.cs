using Godot;
using Miros.Core;

namespace BraveStory;

public class StompGroundEnemyActionState : ActionState
{
    private EnemyContext ctx;

    private Enemy host;
    public override Tag Tag => Tags.State_Action_StompGround;

    public override Tag Layer => Tags.StateLayer_Movement;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Idle, () => ctx.StompTimer >= 1.0f)
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
        host.PlayAnimation("idle"); // 播放跳跃动画
        ctx.StompTimer = 0f;
        ctx.IsStomping = true;

        host.Velocity = new Vector2(0, -200f);
    }

    private void OnPhysicsUpdate(double delta)
    {
        ctx.StompTimer += (float)delta;
        host.MoveAndSlide();

        if (ctx.StompTimer >= ctx.StompDuration)
        {
            host.PlayAnimation("StompBox");
            host.Velocity = new Vector2(0, 800f); // 设置下砸力

            // FIXME: 
            // 1.HItAgent 的获取存在问题
            // 2.HitAgent 需要改为 HitAgent 列表
            if (ctx.HitAgent != null)
            {
                var damageEffect = new Effect
                {
                    Tag = Tags.Effect_Buff,
                    SourceAgent = OwnerAgent,
                    RemovePolicy = RemovePolicy.WhenExited,
                    DurationPolicy = DurationPolicy.Instant,
                    Modifiers =
                    [
                        new Modifier(Tags.Attribute_HP, OwnerAgent.Atr("Attack"), ModifierOperation.Minus,
                            new DamageMMC())
                    ]
                };

                ctx.HitAgent.AddState(damageEffect);

                ctx.HitAgent.SwitchTaskFromTag(ExecutorType.MultiLayerExecutor, Tags.State_Action_Jump,
                    new MultiLayerSwitchArgs(Tags.StateLayer_Movement));

                ctx.HitAgent = null;
            }
        }
    }

    private void OnExit()
    {
        ctx.IsStomping = false;
        ctx.StompTimer = 0f;
    }
}