using System;
using System.Collections.Generic;
using GPC.State;

namespace GPC.Job;

public class JobWrapper
{
    private static readonly Dictionary<Type, IJob> Jobs = new();

    private static IJob _GetJob(Type type)
    {
        if (Jobs.TryGetValue(type, out var value)) return value;
        var job = (IJob)Activator.CreateInstance(type);
        Jobs[type] = job;
        return job;
    }

    public void Enter(AbsState state)
    {
        _GetJob(state.Type).Enter(state);
    }

    public void Exit(AbsState state)
    {
        _GetJob(state.Type).Exit(state);
    }

    public void Pause(AbsState state)
    {
        _GetJob(state.Type).Pause(state);
    }

    public void Resume(AbsState state)
    {
        _GetJob(state.Type).Resume(state);
    }

    public bool IsSucceed(AbsState state)
    {
        return _GetJob(state.Type).IsSucceed(state);
    }

    public bool IsFailed(AbsState state)
    {
        return _GetJob(state.Type).IsFailed(state);
    }

    public bool IsPrepared(AbsState state)
    {
        return _GetJob(state.Type).IsPrepared(state);
    }

    public bool CanExecute(AbsState state)
    {
        return _GetJob(state.Type).CanExecute(state);
    }

    public void Update(AbsState state, double delta)
    {
        _GetJob(state.Type).Update(state, delta);
    }

    public void PhysicsUpdate(AbsState state, double delta)
    {
        _GetJob(state.Type).PhysicsUpdate(state, delta);
    }
}