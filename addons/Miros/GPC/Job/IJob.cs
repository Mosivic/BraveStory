using GPC.States;

namespace GPC.Job;

public interface IJob
{
    void OnStart();
    void OnSucceed();
    void OnFailed();
    void OnPause();
    void OnResume();
    void OnStack(AbsState state);
    void OnStackOverflow(AbsState state);
    bool IsPrepared();
    void Update(double delta);
    void PhysicsUpdate(double delta);
}