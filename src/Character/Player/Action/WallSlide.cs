using Godot;
using Miros.Core;

namespace BraveStory;

public class WallSlideAction : Task<State, Player,PlayerContext>
{
    public override Tag StateTag  => Tags.State_Action_WallSlide;
    public override Tag LayerTag  => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;
    public override Transition[] Transitions  => [
			new (Tags.State_Action_Idle, () => Host.IsOnFloor()),
			new (Tags.State_Action_Fall, () => !Host.IsFootColliding()),
			new (Tags.State_Action_WallJump, () => Host.KeyDownJump()),
		];


  protected override void OnEnter()
  {
    Host.PlayAnimation("wall_sliding");
  }

  protected override void OnPhysicsUpdate(double delta)
  {
    var velocity = Host.Velocity;
    velocity.Y = Mathf.Min(velocity.Y + (float)delta * Agent.Attr("Gravity"), 600);
    Host.Velocity = velocity;

    Host.UpdateFacing(0);
    Host.MoveAndSlide();
  }
}

