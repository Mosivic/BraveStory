namespace Miros.Core;

public interface IStateComponent<T> where T : AbsState<T>
{
    void RegisterHandler(T state);
    void UnregisterHandler(T state);
}
