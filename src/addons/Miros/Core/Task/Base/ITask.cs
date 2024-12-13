namespace Miros.Core;

public interface ITask
{
    void Enter();
    void Exit();
    bool CanEnter();
    bool CanExit();
    void TriggerOnAdd();
    void TriggerOnRemove();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}