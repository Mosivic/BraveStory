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

    public void Enter(AbsState state)
    {
        _GetJob(state.Type).Enter();
    }

    public void Exit(AbsState state)
    {
        _GetJob(state.Type).Exit();
    }

    public void Pause(AbsState state)
    {
        _GetJob(state.Type).Pause();
    }

    public void Resume(State state)
    {
        _GetJob(state.Type).Resume();
    }

    public bool IsSucceed(AbsState state)
    {
        return _GetJob(state.Type).IsSucceed();
    }

    public bool IsFailed(AbsState state)
    {
        return _GetJob(state.Type).IsFailed();
    }

    public bool IsPrepared(AbsState state)
    {
        return _GetJob(state.Type).IsPrepared();
    }

    public bool CanExecute(AbsState state)
    {
        return _GetJob(state.Type).CanExecute();
    }

    public void Update(AbsState state, double delta)
    {
        _GetJob(state.Type).Update(delta);
    }

    public void PhysicsUpdate(AbsState state, double delta)
    {
        _GetJob(state.Type).PhysicsUpdate(delta);
    }
}