namespace Miros.Core;
public interface IStateComponentHandler<T> where T : AbsState<T>
{
    void Initialize(AbsJob job);
    void Cleanup();
}