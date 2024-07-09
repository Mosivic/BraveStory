using GPC.States;

namespace GPC.Job.Executor;

public interface IJobExecutor
{
    void Start(AbsState state);
    void Over(AbsState state);
    void Pause(AbsState state);
    void Resume(AbsState state);
    void Stack(AbsState originState,AbsState stackState);
    bool IsPrepared(AbsState state);
    void Update(AbsState state, double delta);
    void PhysicsUpdate(AbsState state, double delta);
}