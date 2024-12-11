using Godot;
using Miros.Core;

namespace BraveStory;

public partial class ChargeEnemyAction : StateNode<State, Enemy,EnemyShared>
{
    // FIXME：Charge 和 Run 是同一个状态，需要合并
    public override Tag StateTag => Tags.State_Action_Run;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    public override Transition[] Transitions => [
        new (Tags.State_Action_Idle, () => Shared.ChargeTimer >= Shared.ChargeDuration),
        new (Tags.State_Action_Stun, () => Shared.IsStunned)
    ];

    [Export]
    private float _chargeDuration = 0.5f;
    private float _chargeTimer;
    private bool _isCharging;

    protected override void Enter()
    {
        Host.PlayAnimation("run");
        Shared.ChargeTimer = 0f;
        Shared.IsCharging = true;
    }

    protected override void PhysicsUpdate(double delta)
    {
        Charge(delta);

        if(Shared.IsHit && Shared.HitAgentNode != null)
        {
            var damageEffect = new Effect()
            {
                Tag = Tags.Effect_Buff,
                Source = Agent,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new DamageExecution()]
            };

            Shared.HitAgentNode.AddState(ExecutorType.EffectExecutor, damageEffect);
            Shared.IsHit = false;
        }
    }

    protected override void Exit()
    {
        Shared.IsCharging = false;
        Shared.ChargeTimer = 0f;
    }

    private void Charge(double delta)
    {
        // 更新冲刺计时器
        _chargeTimer += (float)delta;

        // 检查是否撞墙
        if (Host.IsWallColliding())
        {
            Shared.IsStunned = true;
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