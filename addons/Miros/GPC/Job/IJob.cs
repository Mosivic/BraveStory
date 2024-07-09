using GPC.States;

namespace GPC.Job;

public interface IJob
{
    void Start();
    void Over();
    void Pause();
    void Resume();
    void Stack(AbsState state);
    bool IsPrepared();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}