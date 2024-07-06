using GPC.Job.Executor;
using GPC.Scheduler;
using GPC.States;

namespace GPC.Job;

internal class JobAll(CompoundState state) : AbsJob(state), IJob
{
    private readonly JobExecutorProvider<StaticJobExecutor> _provider = new();

    public void Enter()
    {
        state.RunningStatus = JobRunningStatus.Running;
        State.RunningResult = JobRunningResult.NoResult;
        
        foreach (var childCfg in state.SubJobs) 
            _provider.Executor.Enter(childCfg);
        _Enter();
        state.EnterAttachFunc?.Invoke(state);
    }


    public void Exit()
    {
        state.RunningStatus = JobRunningStatus.NoRunning;
        State.RunningResult = JobRunningResult.NoResult;
        
        foreach (var childCfg in state.SubJobs) 
            _provider.Executor.Exit(childCfg);
        state.ExitAttachFunc?.Invoke(state);
    }


    public void Pause()
    {
        state.RunningStatus = JobRunningStatus.NoRunning;
        state.PauseAttachFunc?.Invoke(state);
    }


    public void Resume()
    {
        state.RunningStatus = JobRunningStatus.Running;
        state.ResumeAttachFunc?.Invoke(state);
    }


    public bool IsSucceed()
    {
        foreach (var childCfg in state.SubJobs)
        {
            if (childCfg.RunningResult != JobRunningResult.Succeed) return false;
            _provider.Executor.Enter(childCfg);
        }

        return true;
    }


    public bool IsFailed()
    {
        foreach (var childCfg in state.SubJobs)
            if (childCfg.RunningResult == JobRunningResult.Failed)
                return true;
        return false;
    }


    public bool IsPrepared()
    {
        return _IsPrepared();
    }

    public bool CanExecute()
    {
        return IsPrepared();
    }

    public void Update(double delta)
    {
        if (state.RunningStatus == JobRunningStatus.NoRunning) return;

        foreach (var childCfg in state.SubJobs)
            _provider.Executor.Update(childCfg, delta);
        _UpdateJob();
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.PhysicsUpdate(childCfg, delta);
    }

    public void IntervalUpdate()
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.IntervalUpdate(childCfg);
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