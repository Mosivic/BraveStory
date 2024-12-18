using Godot;
using Miros.Core;

namespace BraveStory;

public class ChargeEnemyActionState : ActionState
{
    public override Tag Tag => Tags.State_Action_Charge;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override TaskType TaskType => TaskType.Serial;

    private float _waitTime = 0.3f;
    private AnimatedSprite2D _smoke;

    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => ctx.ChargeTimer >= ctx.ChargeDuration),
        new(Tags.State_Action_Stun, () => ctx.IsStunned)
    ];

    private Enemy host;
    private EnemyContext ctx;

    public override void Init()
    {
        ctx = Context as EnemyContext;
        host = ctx.Host;

        SubStates = [
            new State
            {
                EnterFunc = () => {
                    GD.Print("Boar : ChargeWait");
                }
            },
            new State
            {
                EnterFunc = OnEnter,
                UpdateFunc = OnPhysicsUpdate,
                ExitFunc = OnExit
            }
        ];

        AddFunc = OnAdd;
    }


    private void OnEnter()
    {
        host.PlayAnimation("run");
        
        _waitTime = 1.0f;
        ctx.ChargeTimer = 0f;
        ctx.IsCharging = true;
        
        _smoke.Visible = true;
        _smoke.Play("smoke");

    }
    private void OnAdd()
    {
        var smokeEffect = GD.Load<PackedScene>("res://VFX/smoke.tscn");
        _smoke = smokeEffect.Instantiate<AnimatedSprite2D>();
        host.AddChild(_smoke);
        _smoke.Visible = false;
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (_waitTime > 0)
        {
            _waitTime -= (float)delta;
            return;
        }

        Charge(delta);

        if (ctx.IsHit && ctx.HitAgent != null)
        {
            var damageEffect = new Effect
            {
                Tag = Tags.Effect_Buff,
                SourceAgent = OwnerAgent,
                RemovePolicy = RemovePolicy.WhenExited,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new DamageExecution()]
            };

            ctx.HitAgent.AddEffect(damageEffect);
            ctx.IsHit = false;
        }
    }

    private void OnExit()
    {
        ctx.IsCharging = false;
        ctx.ChargeTimer = 0f;

        _smoke.Visible = false;
        _smoke.Stop();

    }

    private void Charge(double delta)
    {
        // 更新冲刺计时器
        ctx.ChargeTimer += (float)delta;

        // 检查是否撞墙
        if (host.IsWallColliding())
        {
            ctx.IsStunned = true;
            return;
        }

        var velocity = host.Velocity;
        velocity.X = -host.Graphics.Scale.X * OwnerAgent.Atr("RunSpeed") * 2.0f; // 使用当前朝向决定冲刺方向

        // 在冲刺即将结束时减速
        var slowdownThreshold = 0.3f; // 最后0.3秒开始减速
        if (ctx.ChargeTimer >= ctx.ChargeDuration - slowdownThreshold)
        {
            var slowdownFactor = 1.0f - (ctx.ChargeTimer - (ctx.ChargeDuration - slowdownThreshold)) /
                slowdownThreshold;
            velocity.X *= slowdownFactor;
        }

        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        host.Velocity = velocity;
        host.MoveAndSlide();
    }
}