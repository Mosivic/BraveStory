using System;
using System.Collections.Generic;
using System.Reflection;
using GPC.Job.Config;

namespace GPC.Job;

public class JobWrapper
{
    private static readonly Dictionary<Type, IJob> Jobs = new();

    private static IJob _GetJob(Type type)
    {
        if (Jobs.TryGetValue(type, out var value)) return value;
        var job =  (IJob)Activator.CreateInstance(type);
        Jobs[type] = job;
        return job;
    }

    public void Enter(State state)
    {
        _GetJob(state.Type).Enter(state);
    }

    public void Exit(State state)
    {
        _GetJob(state.Type).Exit(state);
    }

    public void Pause(State state)
    {
        _GetJob(state.Type).Pause(state);
    }

    public void Resume(State state)
    {
        _GetJob(state.Type).Resume(state);
    }

    public bool IsSucceed(State state)
    {
        return _GetJob(state.Type).IsSucceed(state);
    }

    public bool IsFailed(State state)
    {
        return _GetJob(state.Type).IsFailed(state);
    }

    public bool IsPrepared(State state)
    {
        return _GetJob(state.Type).IsPrepared(state);
    }

    public bool CanExecute(State state)
    {
        return _GetJob(state.Type).CanExecute(state);
    }

    public void Update(State state, double delta)
    {
        _GetJob(state.Type).Update(state, delta);
    }

    public void PhysicsUpdate(State state, double delta)
    {
        _GetJob(state.Type).PhysicsUpdate(state, delta);
    }
}