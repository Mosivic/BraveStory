using Godot;
using GPC.States;

namespace GPC.Job;

internal class JobSingle(AbsState state) : AbsJob(state), IJob
{
    public void Enter()
    {
#if DEBUG
        GD.Print($"{State.Name} Enter.");
#endif
        state.IsRunning = true;
        State.RunningResult = JobRunningResult.NoResult;
        _Enter();
        State.EnterAttachFunc?.Invoke(State);
    }

    public void Exit()
    {
#if DEBUG
        GD.Print($"{State.Name} Exit.");
#endif
        state.IsRunning = false;
        State.RunningResult = JobRunningResult.NoResult;
        _Exit();
        State.ExitAttachFunc?.Invoke(State);
    }

    public void Pause()
    {
        state.IsRunning = false;
        _Pause();
        State.PauseAttachFunc?.Invoke(State);
    }

    public void Resume()
    {
        state.IsRunning = true;
        _Resume();
        State.ResumeAttachFunc?.Invoke(State);
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

    public void Update(double delta)
    {
        if (!state.IsRunning) return;

        _Update(delta);
        State.RunningAttachFunc?.Invoke(State);

        State.IntervalElapsedTime += delta;
        if (State.IntervalElapsedTime > State.IntervalTime)
        {
            IntervalUpdate();
            State.IntervalElapsedTime = 0;
        }

        _UpdateJob();
    }

    public void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
        State.RunningPhysicsAttachFunc?.Invoke(State);
    }

    public void IntervalUpdate()
    {
        _IntervalUpdate();
        State.AttachIntervalUpdateFunc?.Invoke(State);
    }


    private void _UpdateJob()
    {
        if (IsFailed())
        {
            state.IsRunning = false;
            State.RunningResult = JobRunningResult.Failed;
            //applyEffect()
        }
        else if (IsSucceed())
        {
            State.RunningResult = JobRunningResult.Succeed;
            //applyEffect()
        }
    }
}