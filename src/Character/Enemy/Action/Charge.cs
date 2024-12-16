using Godot;
using Miros.Core;

namespace BraveStory;

public class ChargeEnemyAction : Task<State, Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    public override Tag StateTag => Tags.State_Action_Charge;

    private float _waitTime = 0.3f;
    private AnimatedSprite2D _smoke;
    private bool _IsCharging = false;
    
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () => Context.ChargeTimer >= Context.ChargeDuration),
            new(Tags.State_Action_Stun, () => Context.IsStunned)
        ]
    );

    protected override void OnEnter()
    {
        Host.PlayAnimation("run");

        _waitTime = 1.0f;
        Context.ChargeTimer = 0f;
        Context.IsCharging = true;
        
        _smoke.Visible = true;
        _smoke.Play("smoke");

    }
    protected override void OnAdd()
    {
        var smokeEffect = GD.Load<PackedScene>("res://VFX/smoke.tscn");
        _smoke = smokeEffect.Instantiate<AnimatedSprite2D>();
        Host.AddChild(_smoke);
        _smoke.Visible = false;
    }

    protected override void OnPhysicsUpdate(double delta)
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
                SourceAgent = Agent,
                RemovePolicy = RemovePolicy.WhenExited,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new DamageExecution()]
            };

            Context.HitAgent.AddTaskFromState(ExecutorType.EffectExecutor, damageEffect);
            Context.IsHit = false;
        }
    }

    protected override void OnExit()
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
        if (Host.IsWallColliding())
        {
            Context.IsStunned = true;
            return;
        }

        var velocity = Host.Velocity;
        velocity.X = -Host.Graphics.Scale.X * Agent.Atr("RunSpeed") * 2.0f; // 使用当前朝向决定冲刺方向

        // 在冲刺即将结束时减速
        var slowdownThreshold = 0.3f; // 最后0.3秒开始减速
        if (Context.ChargeTimer >= Context.ChargeDuration - slowdownThreshold)
        {
            var slowdownFactor = 1.0f - (Context.ChargeTimer - (Context.ChargeDuration - slowdownThreshold)) /
                slowdownThreshold;
            velocity.X *= slowdownFactor;
        }

        velocity.Y += (float)delta * Agent.Atr("Gravity");
        Host.Velocity = velocity;
        Host.MoveAndSlide();
    }
}