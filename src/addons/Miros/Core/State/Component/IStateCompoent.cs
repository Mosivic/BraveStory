namespace Miros.Core;

public interface IStateComponent<TJob> where TJob : NativeJob
{
    void Active(TJob job);
    void DeActive(TJob job);
}
