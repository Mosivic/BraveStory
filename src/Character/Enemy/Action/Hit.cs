using Miros.Core;

namespace BraveStory;

public partial class HitEnemyAction : StateNode<State, Enemy>
{
    public override Tag StateTag => Tags.State_Action_Hit;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    protected override void ShareRes()
    {
        Res["Hurt"] = false;
        Res["KnockbackVelocity"] = 50.0f;
    }
    protected override void Enter()
    {
        Res["Hurt"] = false;
        Host.PlayAnimation("hit");
        // 方式1：根据玩家位置计算击退方向
        var playerPos = Host.IsPlayerColliding() ? Host.GetPlayer().GlobalPosition : null;
        if (playerPos.HasValue)
        {
            var direction = (Host.GlobalPosition - playerPos.Value).Normalized();
            Res["Hurt"] = direction.X * Res["Hurt"]; // 击退力度
        }
    }

    protected override void PhysicsUpdate(double delta)
    {
        // 应用击退力
        var velocity = Host.Velocity;
        velocity.X += Res["KnockbackVelocity"];
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        // 逐渐减弱击退效果
        Res["KnockbackVelocity"] *= 0.8f;
        Host.Velocity = velocity;
        Host.MoveAndSlide();        
    }

    protected override void Exit()
    {
        Res["Hurt"] = false;
    }
}
