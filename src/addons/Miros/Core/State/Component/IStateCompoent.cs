namespace Miros.Core;

public interface IStateComponent<TJob> where TJob : NativeJob
{
    void Activate(TJob job);
    void Deactivate(TJob job);
}
