using GPC.Job.Config;

namespace GPC.Job;

internal class JobAll : AbsJob
{
    private readonly JobWrapper _jobJobWrapper = new();

    public void Enter(State cfg)
    {
        cfg.Status = Status.Running;
        foreach (var childCfg in cfg.Subjobs) _jobJobWrapper.Enter(childCfg);
        _Enter(cfg);
        cfg.EnterAttachFunc?.Invoke(cfg);
    }


    public void Exit(State cfg)
    {
        foreach (var childCfg in cfg.Subjobs) _jobJobWrapper.Exit(childCfg);
        cfg.ExitAttachFunc?.Invoke(cfg);
    }


    public void Pause(State cfg)
    {
        cfg.Status = Status.Pause;
        cfg.PauseAttachFunc?.Invoke(cfg);
    }


    public void Resume(State cfg)
    {
        cfg.Status = Status.Running;
        cfg.ResumeAttachFunc?.Invoke(cfg);
    }


    public bool IsSucceed(State cfg)
    {
        foreach (var childCfg in cfg.Subjobs)
        {
            if (childCfg.Status != Status.Successed) return false;
            _jobJobWrapper.Enter(childCfg);
        }

        return true;
    }


    public bool IsFailed(State cfg)
    {
        foreach (var childCfg in cfg.Subjobs)
            if (childCfg.Status == Status.Failed)
                return true;
        return false;
    }


    public bool IsPrepared(State cfg)
    {
        return _IsPrepared(cfg);
    }

    public bool CanExecute(State cfg)
    {
        return IsPrepared(cfg);
    }

    public void Update(State cfg, double delta)
    {
        if (cfg.Status == Status.Pause) return;

        foreach (var childCfg in cfg.Subjobs)
            _jobJobWrapper.Update(childCfg, delta);
        _UpdateJob(cfg);
    }

    public void PhysicsUpdate(State cfg, double delta)
    {
        foreach (var childCfg in cfg.Subjobs)
            _jobJobWrapper.PhysicsUpdate(childCfg, delta);
    }


    private void _UpdateJob(State cfg)
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