using Godot;
using GPC.States;

namespace GPC.Job;

internal class JobSingle(State state) : AbsJob(state), IJob
{
    public void Enter()
    {
#if DEBUG
        GD.Print($"{state.Name} Enter.");
#endif
        state.Status = Status.Running;
        _Enter();
        state.EnterAttachFunc?.Invoke(state);
    }

    public void Exit()
    {
#if DEBUG
        GD.Print($"{state.Name} Exit.");
#endif
        _Exit();
        state.ExitAttachFunc?.Invoke(state);
    }

    public void Pause()
    {
        state.Status = Status.Pause;
        _Pause();
        state.PauseAttachFunc?.Invoke(state);
    }

    public void Resume()
    {
        state.Status = Status.Running;
        _Resume();
        state.ResumeAttachFunc?.Invoke(state);
    }

    public bool IsSucceed()
    {
        return _IsSucceed();
    }

    public bool IsPrepared()
    {
        return _IsPrepared();
    }

    public bool IsFailed()
    {
        return _IsFailed();
    }

    public bool CanExecute()
    {
        return _IsPrepared();
    }

    public void Update(double delta)
    {
        if (state.Status == Status.Pause) return;

        _Update(delta);
        state.RunningAttachFunc?.Invoke(state);
        _UpdateJob();
    }

    public void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
        state.RunningPhysicsAttachFunc?.Invoke(state);
    }


    private void _UpdateJob()
    {
        if (IsFailed())
            state.Status = Status.Failed;
        //applyEffect()
        else if (IsSucceed())
            state.Status = Status.Successed;
        //applyEffect()
        else
            state.Status = Status.Running;
    }
}