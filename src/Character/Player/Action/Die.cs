using Miros.Core;

namespace BraveStory;

public partial class DieAction : StateNode<Player>
{
    protected override Tag StateTag { get; init; } = Tags.State_Status_Die;

    protected override void Enter()
    {
        Host.PlayAnimation("die");
        Host.ClearInteractions();
    }

    protected override void PhysicsUpdate(double delta)
    {
        if (Host.IsAnimationFinished())
        {
            Host.QueueFree();
        }
    }
}
