using System;
using System.Collections.Generic;
using GPC.Job.Config;

namespace GPC.Job;

public class JobWrapper<T> where T : IState
{
    private static readonly Dictionary<Type, IJob<T>> Jobs = new();

    private static IJob<T> _GetJob(Type type)
    {
        if (Jobs.TryGetValue(type, out var value)) return value;
        var job = (IJob<T>)Activator.CreateInstance(type);
        Jobs[type] = job;
        return job;
    }

    public void Enter(T state)
    {
        _GetJob(state.Type).Enter(state);
    }

    public void Exit(T state)
    {
        _GetJob(state.Type).Exit(state);
    }

    public void Pause(T state)
    {
        _GetJob(state.Type).Pause(state);
    }

    public void Resume(T state)
    {
        _GetJob(state.Type).Resume(state);
    }

    public bool IsSucceed(T state)
    {
        return _GetJob(state.Type).IsSucceed(state);
    }

    public bool IsFailed(T state)
    {
        return _GetJob(state.Type).IsFailed(state);
    }

    public bool IsPrepared(T state)
    {
        return _GetJob(state.Type).IsPrepared(state);
    }

    public bool CanExecute(T state)
    {
        return _GetJob(state.Type).CanExecute(state);
    }

    public void Update(T state, double delta)
    {
        _GetJob(state.Type).Update(state, delta);
    }

    public void PhysicsUpdate(T state, double delta)
    {
        _GetJob(state.Type).PhysicsUpdate(state, delta);
    }
}