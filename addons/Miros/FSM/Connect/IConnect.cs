using FSM.States;

namespace FSM.Job.Executor;

public interface IConnect
{
    void AddState(AbsState state);
    void RemoveState(AbsState state);
    void Update(double delta);
    void PhysicsUpdate(double delta);
    bool HasStateRunning(AbsState state);
    bool HasAnyStateRunning(AbsState[] states);
    bool HasAllStateRunning(AbsState[] states);
    IJob[] GetAllJobs();
    bool HasJobRunning(IJob job);
}