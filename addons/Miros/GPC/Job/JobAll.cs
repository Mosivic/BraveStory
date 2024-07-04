using GPC.States;

namespace GPC.Job;

internal class JobAll(State state) : AbsJob(state), IJob
{
    private readonly JobWrapper _jobJobWrapper = new();
    private readonly State _state = state;

    public void Enter()
    {
        _state.Status = Status.Running;
        foreach (var childCfg in _state.SubJobs) _jobJobWrapper.Enter(childCfg);
        _Enter();
        _state.EnterAttachFunc?.Invoke(_state);
    }


    public void Exit()
    {
        foreach (var childCfg in _state.SubJobs) _jobJobWrapper.Exit(childCfg);
        _state.ExitAttachFunc?.Invoke(_state);
    }


    public void Pause()
    {
        _state.Status = Status.Pause;
        _state.PauseAttachFunc?.Invoke(_state);
    }


    public void Resume()
    {
        _state.Status = Status.Running;
        _state.ResumeAttachFunc?.Invoke(_state);
    }


    public bool IsSucceed()
    {
        foreach (var childCfg in _state.SubJobs)
        {
            if (childCfg.Status != Status.Succeed) return false;
            _jobJobWrapper.Enter(childCfg);
        }

        return true;
    }


    public bool IsFailed()
    {
        foreach (var childCfg in _state.SubJobs)
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
        if (_state.Status == Status.Pause) return;

        foreach (var childCfg in _state.SubJobs)
            _jobJobWrapper.Update(childCfg, delta);
        _UpdateJob();
    }

    public void PhysicsUpdate(double delta)
    {
        foreach (var childCfg in _state.SubJobs)
            _jobJobWrapper.PhysicsUpdate(childCfg, delta);
    }


    private void _UpdateJob()
    {
        if (IsFailed())
            _state.Status = Status.Failed;
        //applyEffect()
        else if (IsSucceed())
            _state.Status = Status.Succeed;
        //applyEffect()
        else
            _state.Status = Status.Running;
    }
}