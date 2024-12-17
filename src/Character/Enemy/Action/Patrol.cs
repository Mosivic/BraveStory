using Godot;
using Miros.Core;

namespace BraveStory;

public class PatrolEnemyAction : Action<Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    // FIXME：Walk 和 Patrol 是同一个状态，需要合并
    public override Tag Tag => Tags.State_Action_Patrol;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, () =>
                (!Host.IsFloorColliding() && !Host.IsPlayerColliding() && RunningTime > 2) ||
                (!Host.IsFloorColliding() && Host.IsPlayerColliding())),
            new(Tags.State_Action_Charge, () => Host.IsPlayerColliding())
        ]
    );

    public override void Init(Enemy host, EnemyContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("walk");
    }

    private void OnPhysicsUpdate(double delta)
    {
        Patrol(delta);
    }

    public void Patrol(double delta)
    {
        // 检查是否碰到墙壁
        if (Host.IsWallColliding())
            // 转向：将 X 缩放在 1 和 -1 之间切换
            Host.Graphics.Scale = new Vector2(Host.Graphics.Scale.X * -1, 1);

        // 移动逻辑
        var velocity = Host.Velocity;
        velocity.X = OwnerAgent.Atr("WalkSpeed") * -Host.Graphics.Scale.X; // 注意这里加了负号
        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        Host.Velocity = velocity;
        Host.MoveAndSlide();
    }
}