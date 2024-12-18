using Godot;
using Miros.Core;

namespace BraveStory;

public class HurtEnemyActionState : ActionState<EnemyContext>
{
    public override Tag Tag => Tags.State_Action_Hurt;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => _host.IsAnimationFinished()),
        new(Tags.State_Action_Hurt, () => Context.IsHurt, TransitionMode.Force, 0, true)
    ];

    private Enemy _host;
    public override void Init(EnemyContext context)
    {
        base.Init(context);
        _host = context.Host;

        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        Context.IsHurt = false;
        _host.PlayAnimation("hurt");
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