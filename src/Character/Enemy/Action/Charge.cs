using Godot;
using Miros.Core;

namespace BraveStory;

public partial class ChargeEnemyAction : Task<State, Enemy,EnemyContext>
{
    // FIXME：Charge 和 Run 是同一个状态，需要合并
    public override Tag StateTag => Tags.State_Action_Run;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    public override Transition[] Transitions => [
        new (Tags.State_Action_Idle, () => Context.ChargeTimer >= Context.ChargeDuration),
        new (Tags.State_Action_Stun, () => Context.IsStunned)
    ];

    [Export]
    private float _chargeDuration = 0.5f;
    private float _chargeTimer;
    private bool _isCharging;

    protected override void OnEnter()
    {
        Host.PlayAnimation("run");
        Context.ChargeTimer = 0f;
        Context.IsCharging = true;
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        Charge(delta);

        if(Context.IsHit && Context.HitAgent != null)
        {
            var damageEffect = new Effect()
            {
                Tag = Tags.Effect_Buff,
                Source = Agent,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new DamageExecution()]
            };

            Context.HitAgent.AddState(ExecutorType.EffectExecutor, damageEffect);
            Context.IsHit = false;
        }
    }

    protected override void OnExit()
    {
        Context.IsCharging = false;
        Context.ChargeTimer = 0f;
    }

    private void Charge(double delta)
    {
        // 更新冲刺计时器
        _chargeTimer += (float)delta;

        // 检查是否撞墙
        if (Host.IsWallColliding())
        {
            Context.IsStunned = true;
            return;
        }

        var velocity = Host.Velocity;
        velocity.X = -Host.Graphics.Scale.X * Agent.Attr("RunSpeed") * 2.0f; // 使用当前朝向决定冲刺方向

        // 在冲刺即将结束时减速
        float slowdownThreshold = 0.3f; // 最后0.3秒开始减速
        if (_chargeTimer >= _chargeDuration - slowdownThreshold)
        {
            float slowdownFactor = 1.0f - ((_chargeTimer - (_chargeDuration - slowdownThreshold)) / slowdownThreshold);
            velocity.X *= slowdownFactor;
        }

        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Host.Velocity = velocity;
        Host.MoveAndSlide();
    }
}