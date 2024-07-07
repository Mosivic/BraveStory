using GPC.Job.Executor;
using GPC.States;

namespace GPC.Job;

internal class JobAll(CompoundState state) : JobBase(state)
{
    private readonly JobExecutorProvider<StaticJobExecutor> _provider = new();

    public override void Enter()
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.Enter(childCfg);
        _Enter();
    }


    public override void Exit()
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.Exit(childCfg);
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

    
    public override void Update(double delta)
    {
        if (!state.IsRunning) return;
        
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.Update(childCfg, delta);
  
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.PhysicsUpdate(childCfg, delta);
    }

    public override void IntervalUpdate()
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.IntervalUpdate(childCfg);
    }
    
}