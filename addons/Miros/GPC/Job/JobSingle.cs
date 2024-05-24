using GPC.Job.Config;

namespace GPC.Job;

internal class JobSingle : AbsJob, IJob
{
    public void Enter(State state)
    {
        state.Status = Status.Running;
        _Enter(state);
        state.EnterAttachFunc?.Invoke(state);
    }

    public void Exit(State state)
    {
        _Exit(state);
        state.ExitAttachFunc?.Invoke(state);
    }

    public void Pause(State state)
    {
        state.Status = Status.Pause;
        _Pause(state);
        state.PauseAttachFunc?.Invoke(state);
    }

    public void Resume(State state)
    {
        state.Status = Status.Running;
        _Resume(state);
        state.ResumeAttachFunc?.Invoke(state);
    }

    public bool IsSucceed(State state)
    {
        return _IsSucceed(state);
    }

    public bool IsPrepared(State state)
    {
        return _IsPrepared(state);
    }

    public bool IsFailed(State state)
    {
        return _IsFailed(state);
    }

    public bool CanExecute(State state)
    {
        return _IsPrepared(state);
    }

    public void Update(State state, double delta)
    {
        if (state.Status == Status.Pause) return;

        _Update(state, delta);
        state.RunningAttachFunc?.Invoke(state);
        _UpdateJob(state);
    }

    public void PhysicsUpdate(State state, double delta)
    {
        _PhysicsUpdate(state, delta);
        state.RunningPhysicsAttachFunc?.Invoke(state);
    }


    private void _UpdateJob(State state)
    {
        if (IsFailed(state))
            state.Status = Status.Failed;
        //applyEffect()
        else if (IsSucceed(state))
            state.Status = Status.Successed;
        //applyEffect()
        else
            state.Status = Status.Running;
    }
}