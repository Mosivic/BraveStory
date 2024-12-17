namespace Miros.Core;

public interface ITask<TState>
    where TState : State
{
    void Enter(TState state);
    void Exit(TState state);
    bool CanEnter(TState state);
    bool CanExit(TState state);
    void TriggerOnAdd(TState state);
    void TriggerOnRemove(TState state);
    void Update(TState state, double delta);
    void PhysicsUpdate(TState state, double delta);
}