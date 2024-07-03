using GPC.States;

namespace GPC.Job;

public interface IJob
{
    void Enter();
    void Exit();
    void Pause();
    void Resume();
    bool IsSucceed();
    bool IsPrepared();
    bool IsFailed();
    bool CanExecute();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}