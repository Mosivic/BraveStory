namespace Miros.Core;

public class StateComponent<TJob> where TJob : JobBase
{
    public virtual void Activate(TJob job)
    {
    }

    public virtual void Deactivate(TJob job)
    {
    }
}