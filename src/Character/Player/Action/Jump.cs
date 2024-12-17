using Godot;
using Miros.Core;

namespace BraveStory;

public class JumpActionState : ActionState<Player, PlayerContext, MultiLayerExecuteArgs>
{
    public override Tag Tag => Tags.State_Action_Jump;
    public override MultiLayerExecuteArgs ExecuteArgs => new(
        Tags.StateLayer_Movement,
        [
            new(Tags.State_Action_Fall, () => RunningTime > 0.1f)
        ]
    );

    public override void Init(Player host, PlayerContext context, MultiLayerExecuteArgs executeArgs)
    {
        base.Init(host, context, executeArgs);

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