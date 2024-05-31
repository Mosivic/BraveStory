using GPC.State;

namespace GPC.Job;

public interface IJob
{
    void Enter(IState cfg);
    void Exit(IState cfg);
    void Pause(IState cfg);
    void Resume(IState cfg);
    bool IsSucceed(IState cfg);
    bool IsPrepared(IState cfg);
    bool IsFailed(IState cfg);
    bool CanExecute(IState cfg);
    void Update(IState cfg, double delta);
    void PhysicsUpdate(IState cfg, double delta);
}