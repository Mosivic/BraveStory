using GPC.Job.Executor;
using GPC.Scheduler;
using GPC.States;

namespace GPC.Job;

internal class JobAll(CompoundState state) : AbsJob(state), IJob
{
    public void Enter()
    {
        state.Status = Status.Running;
        foreach (var childCfg in state.SubJobs) _jobJobWrapper.Enter(childCfg);
        _Enter();
        state.EnterAttachFunc?.Invoke(state);
    }


    public void Exit()
    {
        foreach (var childCfg in state.SubJobs) _jobJobWrapper.Exit(childCfg);
        state.ExitAttachFunc?.Invoke(state);
    }


    public void Pause()
    {
        state.Status = Status.Pause;
        state.PauseAttachFunc?.Invoke(state);
    }


    public void Resume()
    {
        state.Status = Status.Running;
        state.ResumeAttachFunc?.Invoke(state);
    }


    public bool IsSucceed()
    {
        foreach (var childCfg in state.SubJobs)
        {
            if (childCfg.Status != Status.Succeed) return false;
            _jobJobWrapper.Enter(childCfg);
        }

        return true;
    }


    public bool IsFailed()
    {
        foreach (var childCfg in state.SubJobs)
            if (childCfg.Status == Status.Failed)
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
        if (state.Status == Status.Pause) return;

        foreach (var childCfg in state.SubJobs)
            _jobJobWrapper.Update(childCfg, delta);
        _UpdateJob();
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var childCfg in state.SubJobs)
            _jobJobWrapper.PhysicsUpdate(childCfg, delta);
    }

    public void IntervalUpdate()
    {
        foreach (var childCfg in state.SubJobs)
            _jobJobWrapper.IntervalUpdate(childCfg);
    }


    private void _UpdateJob()
    {
        if (IsFailed())
            state.Status = Status.Failed;
        //applyEffect()
        else if (IsSucceed())
            state.Status = Status.Succeed;
        //applyEffect()
        else
            state.Status = Status.Running;
    }
}