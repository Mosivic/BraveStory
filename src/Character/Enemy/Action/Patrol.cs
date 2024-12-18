using Godot;
using Miros.Core;

namespace BraveStory;

public class PatrolEnemyActionState : ActionState<EnemyContext>
{
    // FIXME：Walk 和 Patrol 是同一个状态，需要合并
    public override Tag Tag => Tags.State_Action_Patrol;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () =>
            (!_host.IsFloorColliding() && !_host.IsPlayerColliding() && RunningTime > 2) ||
            (!_host.IsFloorColliding() && _host.IsPlayerColliding())),
        new(Tags.State_Action_Charge, () => _host.IsPlayerColliding())
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
        _host.PlayAnimation("walk");
    }

    private void OnPhysicsUpdate(double delta)
    {
        Patrol(delta);
    }

    private void Patrol(double delta)
    {
        // 检查是否碰到墙壁
        if (_host.IsWallColliding())
            // 转向：将 X 缩放在 1 和 -1 之间切换
            _host.Graphics.Scale = new Vector2(_host.Graphics.Scale.X * -1, 1);

        // 移动逻辑
        var velocity = _host.Velocity;
        velocity.X = OwnerAgent.Atr("WalkSpeed") * -_host.Graphics.Scale.X; // 注意这里加了负号
        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        _host.Velocity = velocity;
        _host.MoveAndSlide();
    }
}