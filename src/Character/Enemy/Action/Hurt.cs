using Godot;
using Miros.Core;

namespace BraveStory;

public class HurtEnemyAction : Action<Enemy, EnemyContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Action_Hurt;

    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Idle, Host.IsAnimationFinished),
            new(Tags.State_Action_Hurt, () => Context.IsHurt, TransitionMode.Force, 0, true)
        ]
    );

    public override void Init(Enemy host, EnemyContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);
        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        Context.IsHurt = false;
        Host.PlayAnimation("hurt");
        // // 方式1：根据玩家位置计算击退方向
        // var playerPos = Host.IsPlayerColliding() ? Host.GetPlayer().GlobalPosition : Vector2.Zero;
        // if (playerPos != Vector2.Zero)
        // {
        //     var direction = (Host.GlobalPosition - playerPos).Normalized();
        //     Context.KnockbackVelocity = direction.X * Context.KnockbackVelocity; // 击退力度
        // }
    }

    // protected override void OnPhysicsUpdate(double delta)
    // {
    //     // 应用击退力
    //     var velocity = Host.Velocity;
    //     velocity.X += Context.KnockbackVelocity;
    //     velocity.Y += (float)delta * Agent.Attr("Gravity");
    //     // 逐渐减弱击退效果
    //     Context.KnockbackVelocity *= 0.8f;
    //     Host.Velocity = velocity;
    //     Host.MoveAndSlide();
    // }

}