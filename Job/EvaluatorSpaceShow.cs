using GPC.Job;
using GPC.States;

namespace BraveStory.Job;

internal class UIEvaluator(State state) : JobSingle(state)
{
    protected override void _Enter()
    {
    }

    protected override void _Update( double delta)
    {
        base._Update(delta);
    }
}