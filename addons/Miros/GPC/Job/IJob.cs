namespace GPC.Job;

public interface IJob
{
    void Enter();
    void Exit();
    void Break();
    void Pause();
    void Resume();
    bool IsPrepared();
    void Update(double delta);
    void PhysicsUpdate(double delta);
    void IntervalUpdate();
}