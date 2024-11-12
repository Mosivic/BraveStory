namespace Miros.Core;

public interface IJob
{
    AbsState State { get; }
    void Enter();
    void Exit();
    void Pause();
    void Resume();
    bool CanEnter();
    bool CanExit();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}