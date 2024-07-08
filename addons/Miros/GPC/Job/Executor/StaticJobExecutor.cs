using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Job.Executor;

public class StaticJobExecutor : AbsJobExecutor, IJobExecutor
{
    private static readonly Dictionary<Type, IJob> Jobs = new();

    public void Start(AbsState state)
    {
        _GetJob(state.Type, state).Start();
    }

    public void Succeed(AbsState state)
    {
        _GetJob(state.Type, state).Succeed();
    }

    public void Pause(AbsState state)
    {
        _GetJob(state.Type, state).Pause();
    }

    public void Resume(AbsState state)
    {
        _GetJob(state.Type, state).Resume();
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

    public void Failed(AbsState state)
    {
        _GetJob(state.Type, state).Failed();
    }

    private static IJob _GetJob(Type type, AbsState state)
    {
        if (Jobs.TryGetValue(type, out var value)) return value;
        var job = (IJob)Activator.CreateInstance(type, [state]);
        Jobs[type] = job;
        return job;
    }
}