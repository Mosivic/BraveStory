namespace Miros.Core;

public abstract class BaseComponent<T> where T : AbsState<T>
{
    public abstract void RegisterHandler(T state);
    public abstract void UnregisterHandler(T state);
}