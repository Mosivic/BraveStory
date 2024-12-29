using Godot;
using Miros.Core;

namespace BraveStory;

public class ChargeEnemyActionState : ActionState
{
    private EnemyContext _ctx;

    private Enemy _host;
    private AnimatedSprite2D _smoke;

    public override Tag Tag => Tags.State_Action_Charge;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override TaskType TaskType => TaskType.Serial;


    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Idle, () => Status == RunningStatus.Succeed),
        new(Tags.State_Action_Stun, () => _ctx.IsStunned)
    ];

    public override void Init()
    {
        _ctx = Context as EnemyContext;
        _host = _ctx.Host;


        // 顺序执行以下状态：前摇 0.6f -> 冲击 1.0f -> 后摇 0.3f
        SubStates =
        [
            new DurationState
            {
                Duration = 0.6f,
                EnterFunc = () =>
                {
                    _host.PlayAnimation("idle");
                    _smoke.Visible = true;
                    _smoke.Play("smoke");
                },
                ExitFunc = () =>
                {
                    _smoke.Visible = false;
                    _smoke.Stop();
                }
            },
            new DurationState
            {
                Duration = 1.0f,
                EnterFunc = () => _host.PlayAnimation("run"),
                PhysicsUpdateFunc = OnPhysicsUpdate
            },
            new DurationState
            {
                Duration = 0.3f,
                EnterFunc = () => _host.PlayAnimation("idle")
            }
        ];

        AddFunc = OnAdd;
        ExitFunc = OnExit;
    }


    private void OnAdd()
    {
        var smokeEffect = GD.Load<PackedScene>("res://VFX/smoke.tscn");
        _smoke = smokeEffect.Instantiate<AnimatedSprite2D>();
        _host.AddChild(_smoke);
        _smoke.Visible = false;
    }

    private void OnExit()
    {
        _smoke.Visible = false;
        _smoke.Stop();
    }

    private void OnPhysicsUpdate(double delta)
    {
        Charge(delta);

        if (_ctx.IsHit && _ctx.HitAgent != null)
        {
            var damageEffect = new Effect
            {
                Tag = Tags.Effect_Buff,
                SourceAgent = OwnerAgent,
                RemovePolicy = RemovePolicy.WhenExited,
                DurationPolicy = DurationPolicy.Instant,
                Modifiers =
                [
                    new Modifier(Tags.Attribute_HP, OwnerAgent.Atr("Attack"), ModifierOperation.Minus, new DamageMMC())
                ]
            };

            _ctx.HitAgent.AddEffect(damageEffect);
            _ctx.IsHit = false;
        }
    }


    private void Charge(double delta)
    {
        // 检查是否撞墙
        if (_host.IsWallColliding())
        {
            _ctx.IsStunned = true;
            return;
        }

        var velocity = _host.Velocity;
        velocity.X = -_host.Graphics.Scale.X * OwnerAgent.Atr("RunSpeed") * 3.0f; // 使用当前朝向决定冲刺方向

        // 在冲刺即将结束时减速
        var slowdownThreshold = 0.6f; // 最后0.3秒开始减速
        if (RunningTime >= slowdownThreshold)
        {
            var slowdownFactor = (float)(RunningTime * 2 - slowdownThreshold) /
                                 slowdownThreshold;
            velocity.X *= slowdownFactor;
        }

        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        _host.Velocity = velocity;
        _host.MoveAndSlide();
    }
}