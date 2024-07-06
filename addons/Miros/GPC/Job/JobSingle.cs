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
        State.RunningStatus = JobRunningStatus.Running;
        State.RunningResult = JobRunningResult.NoResult;
        _Enter();
        State.EnterAttachFunc?.Invoke(State);
    }

    public void Exit()
    {
#if DEBUG
        GD.Print($"{State.Name} Exit.");
#endif
        State.RunningStatus = JobRunningStatus.NoRunning;
        State.RunningResult = JobRunningResult.NoResult;
        _Exit();
        State.ExitAttachFunc?.Invoke(State);
    }

    public void Pause()
    {
        State.RunningStatus = JobRunningStatus.NoRunning;
        _Pause();
        State.PauseAttachFunc?.Invoke(State);
    }

    public void Resume()
    {
        State.RunningStatus = JobRunningStatus.Running;
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
        if (State.RunningStatus == JobRunningStatus.NoRunning) return;

        _Update(delta);
        State.RunningAttachFunc?.Invoke(State);

        State.ElapsedTime += delta;
        if (State.ElapsedTime > State.IntervalTime)
        {
            IntervalUpdate();
            State.ElapsedTime = 0;
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
            State.RunningStatus = JobRunningStatus.NoRunning;
            State.RunningResult = JobRunningResult.Failed;
            //applyEffect()
        }
        else if (IsSucceed())
        {
            State.RunningStatus = JobRunningStatus.NoRunning;
            State.RunningResult = JobRunningResult.Succeed;
            //applyEffect()
        }
    }
}