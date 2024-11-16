namespace Miros.Core;

public class StateComponent<TTask> where TTask : TaskBase
{
    public virtual void Activate(TTask task)
    {
    }

    public virtual void Deactivate(TTask task)
    {
    }
}