using Godot;
using Miros.Core;

namespace BraveStory;

public class PatrolEnemyActionState : ActionState
{
    // FIXME：Walk 和 Patrol 是同一个状态，需要合并
    public override Tag Tag => Tags.State_Action_Patrol;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () =>
            (!host.IsFloorColliding() && !host.IsPlayerColliding() && RunningTime > 2) ||
            (!host.IsFloorColliding() && host.IsPlayerColliding())),
        new(Tags.State_Action_Charge, () => host.IsPlayerColliding())
    ];

    private Enemy host;
    private EnemyContext ctx;

    public override void Init()
    {
        ctx = Context as EnemyContext;
        host = ctx.Host;
    
        
        EnterFunc = OnEnter;
        PhysicsUpdateFunc = OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        host.PlayAnimation("walk");
    }

    private void OnPhysicsUpdate(double delta)
    {
        Patrol(delta);
    }

    private void Patrol(double delta)
    {
        // 检查是否碰到墙壁
        if (host.IsWallColliding())
            // 转向：将 X 缩放在 1 和 -1 之间切换
            host.Graphics.Scale = new Vector2(host.Graphics.Scale.X * -1, 1);

        // 移动逻辑
        var velocity = host.Velocity;
        velocity.X = OwnerAgent.Atr("WalkSpeed") * -host.Graphics.Scale.X; // 注意这里加了负号
        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        host.Velocity = velocity;
        host.MoveAndSlide();
    }
}