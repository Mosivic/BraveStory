using Godot;
using GPC.Scheduler;
using GPC.States;

namespace GPC.Job;

internal class JobSingle(AbsState state) : AbsJob(state), IJob
{
    public void Enter()
    {
#if DEBUG
        GD.Print($"{State.Name} Enter.");
#endif
        State.Status = Status.Running;
        _Enter();
        State.EnterAttachFunc?.Invoke(State);
    }

    public void Exit()
    {
#if DEBUG
        GD.Print($"{State.Name} Exit.");
#endif
        State.Status = Status.Pause;
        _Exit();
        State.ExitAttachFunc?.Invoke(State);
    }

    public void Pause()
    {
        State.Status = Status.Pause;
        _Pause();
        State.PauseAttachFunc?.Invoke(State);
    }

    public void Resume()
    {
        State.Status = Status.Running;
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
        if (State.Status == Status.Pause) return;

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
            State.Status = Status.Failed;
        //applyEffect()
        else if (IsSucceed())
            State.Status = Status.Succeed;
        //applyEffect()
        else
            State.Status = Status.Running;
    }
}