using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Job;

public class JobWrapper
{
    private static readonly Dictionary<Type, IJob> Jobs = new();

    private static IJob _GetJob(Type type, AbsState state)
    {
        if (Jobs.TryGetValue(type, out var value)) return value;
        var job = (IJob)Activator.CreateInstance(type, [state]);
        Jobs[type] = job;
        return job;
    }

    public void Enter(AbsState state)
    {
        _GetJob(state.Type, state).Enter();
    }

    public void Exit(AbsState state)
    {
        _GetJob(state.Type, state).Exit();
    }

    public void Pause(AbsState state)
    {
        _GetJob(state.Type, state).Pause();
    }

    public void Resume(CompoundState state)
    {
        _GetJob(state.Type, state).Resume();
    }

    public bool IsSucceed(AbsState state)
    {
        return _GetJob(state.Type, state).IsSucceed();
    }

    public bool IsFailed(AbsState state)
    {
        return _GetJob(state.Type, state).IsFailed();
    }

    public bool IsPrepared(AbsState state)
    {
        return _GetJob(state.Type, state).IsPrepared();
    }

    public void Update(AbsState state, double delta)
    {
        _GetJob(state.Type, state).Update(delta);
    }

    public void PhysicsUpdate(AbsState state, double delta)
    {
        _GetJob(state.Type, state).PhysicsUpdate(delta);
    }
    
    public void IntervalUpdate(AbsState state)
    {
        _GetJob(state.Type, state).IntervalUpdate();
    }
}