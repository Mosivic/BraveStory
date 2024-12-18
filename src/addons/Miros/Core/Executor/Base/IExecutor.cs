namespace Miros.Core;

public interface IExecutor<TState>
    where TState : State
{
    void AddState(TState state);
    void RemoveState(TState state);
    TState GetNowState(Tag layer);
    TState GetLastState(Tag layer);
    TState[] GetAllStates();
    bool HasStateRunning(TState state);
    void Update(double delta);
    void PhysicsUpdate(double delta);
    void SwitchStateByTag(Tag tag, Context context); 
}