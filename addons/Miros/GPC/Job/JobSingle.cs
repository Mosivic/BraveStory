using Godot;
using GPC.State;

namespace GPC.Job;

internal class JobSingle : AbsJob, IJob
{
    public void Enter(AbsState state)
    {
#if DEBUG
        GD.Print($"{state.Name} Enter.");
#endif
        state.Status = Status.Running;
        _Enter(state);
        state.EnterAttachFunc?.Invoke(state);
    }

    public void Exit(AbsState state)
    {
#if DEBUG
        GD.Print($"{state.Name} Exit.");
#endif
        _Exit(state);
        state.ExitAttachFunc?.Invoke(state);
    }

    public void Pause(AbsState state)
    {
        state.Status = Status.Pause;
        _Pause(state);
        state.PauseAttachFunc?.Invoke(state);
    }

    public void Resume(AbsState state)
    {
        state.Status = Status.Running;
        _Resume(state);
        state.ResumeAttachFunc?.Invoke(state);
    }

    public bool IsSucceed(AbsState state)
    {
        return _IsSucceed(state);
    }

    public bool IsPrepared(AbsState state)
    {
        return _IsPrepared(state);
    }

    public bool IsFailed(AbsState state)
    {
        return _IsFailed(state);
    }

    public bool CanExecute(AbsState state)
    {
        return _IsPrepared(state);
    }

    public void Update(AbsState state, double delta)
    {
        if (state.Status == Status.Pause) return;

        _Update(state, delta);
        state.RunningAttachFunc?.Invoke(state);
        _UpdateJob(state);
    }

    public void PhysicsUpdate(AbsState state, double delta)
    {
        _PhysicsUpdate(state, delta);
        state.RunningPhysicsAttachFunc?.Invoke(state);
    }


    private void _UpdateJob(AbsState state)
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