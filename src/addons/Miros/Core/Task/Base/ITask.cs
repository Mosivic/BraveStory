namespace Miros.Core;

public interface ITask
{
    void Enter(State state);
    void Exit(State state);
    bool CanEnter(State state);
    bool CanExit(State state);
    bool CanRemove(State state);
    void TriggerOnAdd(State state);
    void TriggerOnRemove(State state);
    void Update(State state, double delta);
    void PhysicsUpdate(State state, double delta);
    void Stack(State state,bool IsFromSameSource);
}