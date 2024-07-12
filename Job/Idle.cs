using BraveStory.Player;
using BraveStory.State;
using FSM.Job;

internal class Idle(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.Host.AnimationPlayer.Play("idle");
    }
}