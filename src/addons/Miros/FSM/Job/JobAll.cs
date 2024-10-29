using FSM.States;

namespace FSM.Job;

internal class JobAll(CompoundState state) : JobBase(state)
{


    public override void Enter()
    {
        foreach (var childCfg in state.SubJobs)
            _provider.Executor.Enter(childCfg);
        _Enter();
    }


    public bool IsSucceed()
    {
        foreach (var childCfg in state.SubJobs)
        {
            if (childCfg.Status != JobRunningStatus.Succeed) return false;
            _provider.Executor.Enter(childCfg);
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
}