﻿using System;
using System.Collections.Generic;
using System.Linq;
using FSM.States;
using Godot;

namespace FSM.Job.Executor;

public class StaticJobProvider : IJobProvider
{
    private readonly Dictionary<AbsState, IJob> _statesJob = new();

    public IJob GetJob(AbsState state)
    {
        if (_statesJob.TryGetValue(state, out var job))
            return job;
        return CreateJob(state);
    }

    public IJob[] GetAllJobs()
    {
        return _statesJob.Values.ToArray();
    }

    private IJob CreateJob(AbsState state)
    {
        var type = state.JobType;
        var job = (JobSimple)Activator.CreateInstance(type, [state]);
        _statesJob[state] = job;
        return job;
    }
}