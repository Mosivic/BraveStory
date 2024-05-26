using System.Diagnostics;
using Godot;
using GPC.Job.Config;

namespace GPC.Job;


internal class JobSingle<T> :AbsJob<T>,IJob<T> where T : State
{
    public  void Enter(T state)
    {
#if DEBUG
        GD.Print($"{state.Name} Enter.");
#endif
        state.Status = Status.Running;
        _Enter(state);
        state.EnterAttachFunc?.Invoke(state);
    }

    public void Exit(T state)
    {
#if DEBUG
        GD.Print($"{state.Name} Exit.");
#endif
        _Exit(state);
        state.ExitAttachFunc?.Invoke(state);
    }

    public void Pause(T state)
    {
        state.Status = Status.Pause;
        _Pause(state);
        state.PauseAttachFunc?.Invoke(state);
    }

    public void Resume(T state)
    {
        state.Status = Status.Running;
        _Resume(state);
        state.ResumeAttachFunc?.Invoke(state);
    }

    public bool IsSucceed(T state)
    {
        return _IsSucceed(state);
    }

    public bool IsPrepared(T state)
    {
        return _IsPrepared(state);
    }

    public bool IsFailed(T state)
    {
        return _IsFailed(state);
    }

    public bool CanExecute(T state)
    {
        return _IsPrepared(state);
    }

    public void Update(T state, double delta)
    {
        if (state.Status == Status.Pause) return;

        _Update(state, delta);
        state.RunningAttachFunc?.Invoke(state);
        _UpdateJob(state);
    }

    public void PhysicsUpdate(T state, double delta)
    {
        _PhysicsUpdate(state, delta);
        state.RunningPhysicsAttachFunc?.Invoke(state);
    }
    


    private void _UpdateJob(T state)
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