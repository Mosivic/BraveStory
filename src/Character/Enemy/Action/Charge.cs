using Godot;
using Miros.Core;

namespace BraveStory;

public class ChargeEnemyActionState : ActionState
{
    private EnemyContext _ctx;

    private Enemy _host;
    private AnimatedSprite2D _smoke;

    private float _waitTime = 0.3f;
    public override Tag Tag => Tags.State_Action_Charge;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override TaskType TaskType => TaskType.Serial;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Idle, () => _ctx.ChargeTimer >= _ctx.ChargeDuration),
        new(Tags.State_Action_Stun, () => _ctx.IsStunned)
    ];

    public override void Init()
    {
        _ctx = Context as EnemyContext;
        _host = _ctx.Host;

        SubStates =
        [
            new DelayState
            {
                DelayTime = 1.0f,
                EnterFunc = () => { GD.Print("Pre-Cast Delay Start"); },
                ExitFunc = () => { GD.Print("Pre-Cast Delay End"); }
            },
            new State
            {
                EnterFunc = OnEnter,
                PhysicsUpdateFunc = OnPhysicsUpdate,
                ExitFunc = OnExit
            },
            new DelayState
            {
                DelayTime = 1.0f,
                EnterFunc = () => { GD.Print("Post-Cast Delay Start"); },
                ExitFunc = () => { GD.Print("Post-Cast Delay End"); }
            },
        ];

        AddFunc = OnAdd;
    }


    private void OnEnter()
    {
        _host.PlayAnimation("run");

        _waitTime = 1.0f;
        _ctx.ChargeTimer = 0f;
        _ctx.IsCharging = true;

        _smoke.Visible = true;
        _smoke.Play("smoke");
    }

    private void OnAdd()
    {
        var smokeEffect = GD.Load<PackedScene>("res://VFX/smoke.tscn");
        _smoke = smokeEffect.Instantiate<AnimatedSprite2D>();
        _host.AddChild(_smoke);
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

        if (_ctx.IsHit && _ctx.HitAgent != null)
        {
            var damageEffect = new Effect
            {
                Tag = Tags.Effect_Buff,
                SourceAgent = OwnerAgent,
                RemovePolicy = RemovePolicy.WhenExited,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new DamageExecution()]
            };

            _ctx.HitAgent.AddEffect(damageEffect);
            _ctx.IsHit = false;
        }
    }

    private void OnExit()
    {
        _ctx.IsCharging = false;
        _ctx.ChargeTimer = 0f;

        _smoke.Visible = false;
        _smoke.Stop();
    }

    private void Charge(double delta)
    {
        // 更新冲刺计时器
        _ctx.ChargeTimer += (float)delta;

        // 检查是否撞墙
        if (_host.IsWallColliding())
        {
            _ctx.IsStunned = true;
            return;
        }

        var velocity = _host.Velocity;
        velocity.X = -_host.Graphics.Scale.X * OwnerAgent.Atr("RunSpeed") * 2.0f; // 使用当前朝向决定冲刺方向

        // 在冲刺即将结束时减速
        var slowdownThreshold = 0.3f; // 最后0.3秒开始减速
        if (_ctx.ChargeTimer >= _ctx.ChargeDuration - slowdownThreshold)
        {
            var slowdownFactor = 1.0f - (_ctx.ChargeTimer - (_ctx.ChargeDuration - slowdownThreshold)) /
                slowdownThreshold;
            velocity.X *= slowdownFactor;
        }

        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        _host.Velocity = velocity;
        _host.MoveAndSlide();
    }
}