namespace Miros.Core;

public interface IStateComponent<TJob> where TJob : NativeJob
{
    void RegisterHandler(TJob job);
    void UnregisterHandler(TJob job);
}
