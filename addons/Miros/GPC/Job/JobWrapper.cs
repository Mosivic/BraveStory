using System;
using System.Collections.Generic;
using GPC.States;

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

    public void Enter(State state)
    {
        _GetJob(state.Type).Enter();
    }

    public void Exit(State state)
    {
        _GetJob(state.Type).Exit();
    }

    public void Pause(State state)
    {
        _GetJob(state.Type).Pause();
    }

    public void Resume(State state)
    {
        _GetJob(state.Type).Resume();
    }

    public bool IsSucceed(State state)
    {
        return _GetJob(state.Type).IsSucceed();
    }

    public bool IsFailed(State state)
    {
        return _GetJob(state.Type).IsFailed();
    }

    public bool IsPrepared(State state)
    {
        return _GetJob(state.Type).IsPrepared();
    }

    public bool CanExecute(State state)
    {
        return _GetJob(state.Type).CanExecute();
    }

    public void Update(State state, double delta)
    {
        _GetJob(state.Type).Update(delta);
    }

    public void PhysicsUpdate(State state, double delta)
    {
        _GetJob(state.Type).PhysicsUpdate(delta);
    }
}