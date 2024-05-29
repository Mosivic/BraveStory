using GPC.State;

namespace GPC.Job;

public interface IJob<in T> where T : IState
{
    void Enter(T cfg);
    void Exit(T cfg);
    void Pause(T cfg);
    void Resume(T cfg);
    bool IsSucceed(T cfg);
    bool IsPrepared(T cfg);
    bool IsFailed(T cfg);
    bool CanExecute(T cfg);
    void Update(T cfg, double delta);
    void PhysicsUpdate(T cfg, double delta);
}