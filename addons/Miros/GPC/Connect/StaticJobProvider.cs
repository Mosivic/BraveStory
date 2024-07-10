using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Job.Executor;

public class StaticJobProvider : IJobProvider
{
    private static Dictionary<Type, IJob> _jobs  = new();
    private readonly Dictionary<AbsState, IJob> _statesJob = new();
    
    private IJob CreateJob(AbsState state)
    {
        var type = state.Type;
        var job = (IJob)Activator.CreateInstance(type, [state]);
        _jobs[type] = job;
        _statesJob[state] = job;
        return job;
    }
    
    public IJob GetJob(AbsState state)
    {
        if (_statesJob.TryGetValue(state, out var job) )
            return job;
        return CreateJob(state);
    }
}