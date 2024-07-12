using BraveStory.State;
using Godot;
using FSM.Job;

namespace BraveStory.Job;

internal class Test(CharacterState state) : JobBase(state)
{
    protected override void _Enter()
    {
        GD.Print("This is Test.");
    }

    protected override void _Update(double delta)
    {
        GD.Print("This is Test Update.");
    }

    protected override bool _IsPrepared()
    {
        return true;
    }
}