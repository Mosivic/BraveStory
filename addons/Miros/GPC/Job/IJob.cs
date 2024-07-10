using GPC.States;

namespace GPC.Job;

public interface IJob
{
    void Enter();
    void Exit();
    void Pause();
    void Resume();
    void Stack(AbsState state);
    bool CanEnter();
    bool CanExit();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}