using Godot;
using Miros.Core;

namespace BraveStory;

public partial class WallSlideAction : StateNode<Player>
{
  protected override Tag StateTag { get; init; } = Tags.State_Action_WallSlide;

  protected override void Enter()
  {
    Host.PlayAnimation("wall_sliding");
  }

  protected override void PhysicsUpdate(double delta)
  {
    var velocity = Host.Velocity;
    velocity.Y = Mathf.Min(velocity.Y + (float)delta * Agent.Attr("Gravity"), 600);
    Host.Velocity = velocity;

    Host.UpdateFacing(0);
    Host.MoveAndSlide();
  }
}

