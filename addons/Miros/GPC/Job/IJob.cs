namespace GPC.Job;

public interface IJob
{
    string Name { get; }
    Layer Layer { get; }
    int Priority { get; }
    bool IsStack { get; }
    IGpcToken Source { get; }
    void Enter();
    void Exit();
    void Pause();
    void Resume();
    void Stack(IGpcToken source);
    bool CanEnter();
    bool CanExit();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}