namespace BraveStory.Job;

internal class UIEvaluator<T> : JobSingle<T> where T : PlayerState
{
    protected override void _Enter(T state)
    {
    }

    protected override void _Update(T state, double delta)
    {
        base._Update(state, delta);
    }
}