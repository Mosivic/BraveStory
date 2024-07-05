using GPC.States;

namespace GPC.Job.Executor;

public interface IJobExecutor
{
    void Enter(AbsState state);
    void Exit(AbsState state);
    void Pause(AbsState state);
    void Resume(AbsState state);
    bool IsSucceed(AbsState state);
    bool IsPrepared(AbsState state);
    bool IsFailed(AbsState state);
    void Update(AbsState state,double delta);
    void PhysicsUpdate(AbsState state,double delta);
    void IntervalUpdate(AbsState state);
}