using GPC.States;

namespace GPC.Job;

public interface IJob
{
    void Enter(State cfg);
    void Exit(State cfg);
    void Pause(State cfg);
    void Resume(State cfg);
    bool IsSucceed(State cfg);
    bool IsPrepared(State cfg);
    bool IsFailed(State cfg);
    bool CanExecute(State cfg);
    void Update(State cfg, double delta);
    void PhysicsUpdate(State cfg, double delta);
}