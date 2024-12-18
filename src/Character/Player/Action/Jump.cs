using Godot;
using Miros.Core;

namespace BraveStory;

public class JumpActionState : ActionState<Player, PlayerContext>
{
    public override Tag Tag => Tags.State_Action_Jump;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Fall, () => RunningTime > 0.1f)
    ];


    public override void Init(Player host, PlayerContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
        PhysicsUpdateFunc += OnPhysicsUpdate;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Velocity.X, OwnerAgent.Atr("JumpVelocity"));
        Context.JumpCount++;
    }

    private void OnPhysicsUpdate(double delta)
    {
        Host.MoveAndSlide();
    }
}