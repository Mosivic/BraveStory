using System.Collections.Generic;

namespace Miros.Core;

public class Connect<TJobProvider, TScheduler> : IConnect
    where TJobProvider : IJobProvider, new()
    where TScheduler : IScheduler, new()
{
    protected TJobProvider _jobProvider = new();
    protected TScheduler _scheduler = new();

    public Connect(HashSet<AbsState> states)
    {
        foreach(var state in states){
            var job = _jobProvider.GetJob(state);
            _scheduler.AddJob(job);
        }
    }


    public void AddState(AbsState state)
    {
        var job = _jobProvider.GetJob(state);
        _scheduler.AddJob(job);
    }


    public void RemoveState(AbsState state)
    {
        var job = _jobProvider.GetJob(state);
        _scheduler.RemoveJob(job);
    }

    public void Update(double delta)
    {
        _scheduler.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        _scheduler.PhysicsUpdate(delta);
    }


    public IJob[] GetAllJobs()
    {
        return _jobProvider.GetAllJobs();
    }

    public AbsState GetNowState(Tag layer){
        return _scheduler.GetNowState(layer);
    }

    public AbsState GetLastState(Tag layer){
        return _scheduler.GetLastState(layer);
    }

    public double GetCurrentStateTime(Tag layer){
        return _scheduler.GetCurrentStateTime(layer);
    }

}