using System;
using System.Collections.Generic;
using System.Reflection;
using GPC.Job.Config;

namespace GPC.Job;

public class JobWrapper<U> where U : IState 
{
    private static readonly Dictionary<Type, IJob<U>> Jobs = new();

    private static IJob<U> _GetJob(Type type)
    {
        if (Jobs.TryGetValue(type, out var value)) return value;
        var job =  (IJob<U>)Activator.CreateInstance(type);
        Jobs[type] = job ;
        return job;
    }

    public void Enter(U state)
    {
        _GetJob(state.Type).Enter(state);
    }

    public void Exit(U state)
    {
        _GetJob(state.Type).Exit(state);
    }

    public void Pause(U state)
    {
        _GetJob(state.Type).Pause(state);
    }

    public void Resume(U state)
    {
        _GetJob(state.Type).Resume(state);
    }

    public bool IsSucceed(U state)
    {
        return _GetJob(state.Type).IsSucceed(state);
    }

    public bool IsFailed(U state)
    {
        return _GetJob(state.Type).IsFailed(state);
    }

    public bool IsPrepared(U state)
    {
        return _GetJob(state.Type).IsPrepared(state);
    }

    public bool CanExecute(U state)
    {
        return _GetJob(state.Type).CanExecute(state);
    }

    public void Update(U state, double delta)
    {
        _GetJob(state.Type).Update(state, delta);
    }

    public void PhysicsUpdate(U state, double delta)
    {
        _GetJob(state.Type).PhysicsUpdate(state, delta);
    }
}