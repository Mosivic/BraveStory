namespace Miros.Core;
public interface IConnect
{
    void AddState<TState>(TState state)
        where TState : AbsState;
    void RemoveState<TState>(TState state)
        where TState : AbsState;
    void Update(double delta);
    void PhysicsUpdate(double delta);
    IJob[] GetAllJobs();
}