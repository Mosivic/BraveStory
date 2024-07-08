using System;
using System.Collections.Generic;
using GPC.States;

namespace GPC.Job.Executor;

public class StaticJobExecutor : AbsJobExecutor, IJobExecutor
{
    private static readonly Dictionary<Type, IJob> Jobs = new();

    public void Start(AbsState state)
    {
        _GetJob(state.Type, state).OnStart();
    }

    public void Succeed(AbsState state)
    {
        _GetJob(state.Type, state).OnSucceed();
    }

    public void Pause(AbsState state)
    {
        _GetJob(state.Type, state).OnPause();
    }

    public void Resume(AbsState state)
    {
        _GetJob(state.Type, state).OnResume();
    }

    public void Stack(AbsState state)
    {
        _GetJob(state.Type, state).OnStack();
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

    
    public void Failed(AbsState state)
    {
        _GetJob(state.Type, state).OnFailed();
    }

    private static IJob _GetJob(Type type, AbsState state)
    {
        if (Jobs.TryGetValue(type, out var value)) return value;
        var job = (IJob)Activator.CreateInstance(type, [state]);
        Jobs[type] = job;
        return job;
    }
}