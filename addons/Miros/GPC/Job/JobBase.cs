using GPC.States;

namespace GPC.Job;

public abstract class JobBase(AbsState state) :AbsJob(state),IJob
{
    public virtual void Enter()
    {
        State.IsRunning = true;
        State.RunningResult = JobRunningResult.NoResult;
        _Enter();
    }

    public virtual void Exit()
    {
        State.IsRunning = false;
        _Exit();
    }

    public virtual void Break()
    {
        State.IsRunning = false;
        State.RunningResult = JobRunningResult.NoResult;
        _Break();
    }

    public virtual void Pause()
    {
        State.IsRunning = false;
        _Pause();
    }

    public virtual void Resume()
    {
        State.IsRunning = true;
        _Resume();
    }
    
    public virtual bool IsPrepared()
    {
        return _IsPrepared();
    }
    
    
    public virtual void Update(double delta)
    {
        if (!state.IsRunning) return;

        if (_IsFailed() || _IsSucceed())
        {
            state.IsRunning = false;
        }
        
        State.IntervalElapsedTime += delta;
        if (State.IntervalElapsedTime > State.IntervalTime)
        {
            _IntervalUpdate();
            State.IntervalElapsedTime = 0;
        }
        
        _Update(delta);
    }

    public virtual void PhysicsUpdate(double delta)
    {
        if (!state.IsRunning) return;
        _PhysicsUpdate(delta);
    }

    public virtual void IntervalUpdate()
    {
        _IntervalUpdate();
    }
}