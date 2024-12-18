namespace Miros.Core;

public interface IExecutor
{
    void AddState(State state);
    void RemoveState(State state);
    State GetNowState(Tag layer);
    State GetLastState(Tag layer);
    State[] GetAllStates();
    bool HasStateRunning(State state);
    void Update(double delta);
    void PhysicsUpdate(double delta);
    void SwitchStateByTag(Tag tag, Context context); 
}