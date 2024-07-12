using BraveStory.State;
using FSM.Job;

internal class Idle(CharacterState state) : JobBase(state)
{
    protected override void _Enter()
    {
        state.AnimationPlayer.Play("idle");
    }
}