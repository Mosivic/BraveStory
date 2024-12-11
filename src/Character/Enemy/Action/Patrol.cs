using Godot;
using Miros.Core;

namespace BraveStory;

public partial class PatrolEnemyAction : StateNode<State, Enemy>
{
    // FIXME：Walk 和 Patrol 是同一个状态，需要合并
    public override Tag StateTag => Tags.State_Action_Walk;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    protected override void Enter()
    {
        Host.PlayAnimation("walk");
    }

    protected override void PhysicsUpdate(double delta)
    {
        Host.Patrol(delta);
    }

    public void Patrol(double delta)
    {
        // 检查是否碰到墙壁
        if (Host.IsWallColliding())
            // 转向：将 X 缩放在 1 和 -1 之间切换
            Host.Graphics.Scale = new Vector2(Host.Graphics.Scale.X * -1, 1);

        // 移动逻辑
        var velocity = Host.Velocity;
        velocity.X = Agent.Attr("WalkSpeed") * -Host.Graphics.Scale.X; // 注意这里加了负号
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Host.Velocity = velocity;
        Host.MoveAndSlide();
    }
}
