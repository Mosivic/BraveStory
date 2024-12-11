using Godot;
using Miros.Core;

namespace BraveStory;

public partial class WallSlideAction : StateNode<State, Player,PlayerShared>
{
    public override Tag StateTag  => Tags.State_Action_WallSlide;
    public override Tag LayerTag  => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition[] Transitions  => [
			new (Tags.State_Action_Idle, () => Host.IsOnFloor()),
			new (Tags.State_Action_Fall, () => !Host.IsFootColliding()),
			new (Tags.State_Action_WallJump, () => Host.KeyDownJump()),
		];


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

