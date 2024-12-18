using Godot;
using Miros.Core;

namespace BraveStory;

public class ChargeEnemyActionState : ActionState<EnemyContext>
{
    public override Tag Tag => Tags.State_Action_Charge;

    private float _waitTime = 0.3f;
    private AnimatedSprite2D _smoke;
    private bool _IsCharging = false;

    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => Context.ChargeTimer >= Context.ChargeDuration),
        new(Tags.State_Action_Stun, () => Context.IsStunned)
    ];

    private Enemy _host;

    public override void Init(EnemyContext context)
    {
        base.Init(context);
        _host = context.Host;

        AddFunc += OnAdd;
        EnterFunc += OnEnter;
        UpdateFunc += OnPhysicsUpdate;
        ExitFunc += OnExit;
    }


    private void OnEnter()
    {
        Context.Host.PlayAnimation("run");
        
        _waitTime = 1.0f;
        Context.ChargeTimer = 0f;
        Context.IsCharging = true;
        
        _smoke.Visible = true;
        _smoke.Play("smoke");

    }
    private void OnAdd()
    {
        var smokeEffect = GD.Load<PackedScene>("res://VFX/smoke.tscn");
        _smoke = smokeEffect.Instantiate<AnimatedSprite2D>();
        Context.Host.AddChild(_smoke);
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

        if (Context.IsHit && Context.HitAgent != null)
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
            Context.IsHit = false;
        }
    }

    private void OnExit()
    {
        Context.IsCharging = false;
        Context.ChargeTimer = 0f;

        _smoke.Visible = false;
        _smoke.Stop();

    }

    private void Charge(double delta)
    {
        // 更新冲刺计时器
        Context.ChargeTimer += (float)delta;

        // 检查是否撞墙
        if (_host.IsWallColliding())
        {
            Context.IsStunned = true;
            return;
        }

        var velocity = _host.Velocity;
        velocity.X = -_host.Graphics.Scale.X * OwnerAgent.Atr("RunSpeed") * 2.0f; // 使用当前朝向决定冲刺方向

        // 在冲刺即将结束时减速
        var slowdownThreshold = 0.3f; // 最后0.3秒开始减速
        if (Context.ChargeTimer >= Context.ChargeDuration - slowdownThreshold)
        {
            var slowdownFactor = 1.0f - (Context.ChargeTimer - (Context.ChargeDuration - slowdownThreshold)) /
                slowdownThreshold;
            velocity.X *= slowdownFactor;
        }

        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        _host.Velocity = velocity;
        _host.MoveAndSlide();
    }
}