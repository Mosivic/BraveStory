using BraveStory.Player;
using FSM.Job;

internal class Idle(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.Host.AnimationPlayer.Play("idle");
    }
}