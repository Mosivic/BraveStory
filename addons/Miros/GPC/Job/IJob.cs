namespace GPC.Job;

public interface IJob
{
    void Start();
    void Succeed();
    void Failed();
    void Pause();
    void Resume();
    bool IsPrepared();
    void Update(double delta);
    void PhysicsUpdate(double delta);
    void IntervalUpdate();
}