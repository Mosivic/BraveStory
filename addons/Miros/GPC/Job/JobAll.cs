using GPC.State;

namespace GPC.Job;

internal class JobAll<T> : AbsJob<T>, IJob<T> where T : State.State
{
    private readonly JobWrapper<IState> _jobJobWrapper = new();

    public void Enter(T cfg)
    {
        cfg.Status = Status.Running;
        foreach (var childCfg in cfg.Subjobs) _jobJobWrapper.Enter(childCfg);
        _Enter(cfg);
        cfg.EnterAttachFunc?.Invoke(cfg);
    }


    public void Exit(T cfg)
    {
        foreach (var childCfg in cfg.Subjobs) _jobJobWrapper.Exit(childCfg);
        cfg.ExitAttachFunc?.Invoke(cfg);
    }


    public void Pause(T cfg)
    {
        cfg.Status = Status.Pause;
        cfg.PauseAttachFunc?.Invoke(cfg);
    }


    public void Resume(T cfg)
    {
        cfg.Status = Status.Running;
        cfg.ResumeAttachFunc?.Invoke(cfg);
    }


    public bool IsSucceed(T cfg)
    {
        foreach (var childCfg in cfg.Subjobs)
        {
            if (childCfg.Status != Status.Successed) return false;
            _jobJobWrapper.Enter(childCfg);
        }

        return true;
    }


    public bool IsFailed(T cfg)
    {
        foreach (var childCfg in cfg.Subjobs)
            if (childCfg.Status == Status.Failed)
                return true;
        return false;
    }


    public bool IsPrepared(T cfg)
    {
        return _IsPrepared(cfg);
    }

    public bool CanExecute(T cfg)
    {
        return IsPrepared(cfg);
    }

    public void Update(T cfg, double delta)
    {
        if (cfg.Status == Status.Pause) return;

        foreach (var childCfg in cfg.Subjobs)
            _jobJobWrapper.Update(childCfg, delta);
        _UpdateJob(cfg);
    }

    public void PhysicsUpdate(T cfg, double delta)
    {
        foreach (var childCfg in cfg.Subjobs)
            _jobJobWrapper.PhysicsUpdate(childCfg, delta);
    }


    private void _UpdateJob(T cfg)
    {
        if (IsFailed(cfg))
            cfg.Status = Status.Failed;
        //applyEffect()
        else if (IsSucceed(cfg))
            cfg.Status = Status.Successed;
        //applyEffect()
        else
            cfg.Status = Status.Running;
    }
}