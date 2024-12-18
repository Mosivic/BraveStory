using Godot;
using Miros.Core;

namespace BraveStory;

public class FallActionState : ActionState<Player, PlayerContext>
{
    public override Tag Tag => Tags.State_Action_Fall;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => Host.IsOnFloor()),
        new(Tags.State_Action_WallSlide, () => Host.IsHandColliding() && Host.IsFootColliding() && !Host.KeyDownMove()),
        new(Tags.State_Action_Jump, () => Host.KeyDownJump() && Context.JumpCount < Context.MaxJumpCount)
    ];

    public override void Init(Player host, PlayerContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("fall");
    }


    private void OnPhysicsUpdate(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Host.Velocity;

        // 确保空中移动控制合理
        velocity.X = Mathf.MoveToward(
            velocity.X,
            direction * OwnerAgent.Atr("RunSpeed"),
            OwnerAgent.Atr("AirAcceleration")
        );

        velocity.Y += (float)delta * OwnerAgent.Atr("Gravity");
        Host.Velocity = velocity;

        Host.UpdateFacing(direction);
        Host.MoveAndSlide();
    }
}