using Godot;
using Miros.Core;

namespace BraveStory;

public class WallSlideActionState : ActionState<Player, PlayerContext>
{
    public override Tag Tag => Tags.State_Action_WallSlide;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Idle, () => Host.IsOnFloor()),
        new(Tags.State_Action_Fall, () => !Host.IsFootColliding()),
        new(Tags.State_Action_WallJump, () => Host.KeyDownJump())
    ];

    public override void Init(Player host, PlayerContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("wall_sliding");
    }

    private void OnPhysicsUpdate(double delta)
    {
        var velocity = Host.Velocity;
        velocity.Y = Mathf.Min(velocity.Y + (float)delta * OwnerAgent.Atr("Gravity"), 600);
        Host.Velocity = velocity;

        Host.UpdateFacing(0);
        Host.MoveAndSlide();
    }
}