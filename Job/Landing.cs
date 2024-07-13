using BraveStory.Player;
using FSM.Job;

internal class Landing(PlayerState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.Host.AnimationPlayer.Play("landing");
    }
}