using BraveStory.State;
using FSM.Job;

internal class Landing(CharacterState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.AnimationPlayer.Play("landing");
    }
}