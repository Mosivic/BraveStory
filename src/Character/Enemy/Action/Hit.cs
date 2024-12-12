
using Godot;
using Miros.Core;

namespace BraveStory;

public  class HitEnemyAction : Task<State, Enemy,EnemyContext>
{
    public override Tag StateTag => Tags.State_Action_Hit;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    public override Transition[] Transitions => [
        new (Tags.State_Action_Idle, Host.IsAnimationFinished),
        new (Tags.State_Action_Hit, () => Context.IsHurt,TransitionMode.Force,0,true)
    ];


    protected override void OnEnter()
    {
        Context.IsHurt = false;
        Host.PlayAnimation("hit");
        // 方式1：根据玩家位置计算击退方向
        var playerPos = Host.IsPlayerColliding() ? Host.GetPlayer().GlobalPosition : Vector2.Zero;
        if (playerPos != Vector2.Zero)
        {
            var direction = (Host.GlobalPosition - playerPos).Normalized();
            Context.KnockbackVelocity = direction.X * Context.KnockbackVelocity; // 击退力度
        }
    }

    protected override void OnPhysicsUpdate(double delta)
    {
        // 应用击退力
        var velocity = Host.Velocity;
        velocity.X += Context.KnockbackVelocity;
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        // 逐渐减弱击退效果
        Context.KnockbackVelocity *= 0.8f;
        Host.Velocity = velocity;
        Host.MoveAndSlide();        
    }

    protected override void OnExit()
    {
        Context.IsHurt = false;
    }
}