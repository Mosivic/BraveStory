namespace Miros.Core;

public interface IJob
{
    void Enter();
    void Exit();
    void Pause();
    void Resume();
    bool CanEnter();
    bool CanExit();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}