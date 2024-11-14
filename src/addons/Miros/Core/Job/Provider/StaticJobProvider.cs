using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class StaticJobProvider : IJobProvider
{
    private readonly Dictionary<State, JobBase> _statesJob = [];

    public JobBase GetJob(State state)
    {
        if (_statesJob.TryGetValue(state, out var job))
            return job;
        return CreateJob(state);
    }

    public JobBase[] GetAllJobs()
    {
        return _statesJob.Values.ToArray();
    }

    private JobBase CreateJob(State state)
    {
        var type = state.JobType;
        var job = (JobBase)Activator.CreateInstance(type, state);
        _statesJob[state] = job;
        return job;
    }
}