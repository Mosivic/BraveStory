namespace GPC.Job.Executor;

public interface IJobExecutor
{
    void Enter();
    void Exit();
    void Pause();
    void Resume();
    bool IsSucceed();
    bool IsPrepared();
    bool IsFailed();
    void Update(double delta);
    void PhysicsUpdate(double delta);
    void IntervalUpdate();
}