using GPC.Job.Executor;
using GPC.States;

namespace GPC.Job;

internal class JobAll(CompoundState state) : JobBase(state)
{
    private readonly JobExecutorProvider<StaticJobExecutor> _provider = new();

    public override void Start()
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.Start(childCfg);
        _Start();
    }


    public override void Succeed()
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.Succeed(childCfg);
    }


    
    public bool IsSucceed()
    {
        foreach (var childCfg in state.SubJobs)
        {
            if (childCfg.Status != JobRunningStatus.Succeed) return false;
            _provider.Executor.Start(childCfg);
        }

        return true;
    }


    public bool IsFailed()
    {
        foreach (var childCfg in state.SubJobs)
            if (childCfg.Status == JobRunningStatus.Failed)
                return true;
        return false;
    }

    
    public override void Update(double delta)
    {
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