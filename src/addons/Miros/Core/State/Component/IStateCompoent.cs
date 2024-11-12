namespace Miros.Core;

public interface IStateComponent<TJob> where TJob : JobBase
{
    void Activate(TJob job);
    void Deactivate(TJob job);
}
