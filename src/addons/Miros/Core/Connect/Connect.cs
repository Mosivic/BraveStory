﻿using System;
using System.Collections.Generic;

namespace Miros.Core;

public class Connect<TJobProvider> : IConnect
    where TJobProvider : IJobProvider, new()
{
    protected TJobProvider _jobProvider = new();
    protected Dictionary<Type,IScheduler<JobBase>> _schedulers = [];

    
    public void AddScheduler<TState>(IScheduler<JobBase> scheduler,HashSet<TState> states)
        where TState : AbsState
    {
        _schedulers[typeof(TState)] = scheduler;
        foreach(var state in states){
            var job = _jobProvider.GetJob(state);
            scheduler.AddJob(job);
        }
    }

    public void AddState<TState>(TState state)
        where TState : AbsState
    {
        var type = state.GetType();
        if(!_schedulers.TryGetValue(type,out var scheduler)){
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] scheduler of {type} not found");
#else
            return;
 #endif
        }
        var job = _jobProvider.GetJob(state);
        scheduler.AddJob(job);
    }


    public void RemoveState<TState>(TState state)
        where TState : AbsState
    {
        var type = state.GetType();
        if(!_schedulers.TryGetValue(type,out var scheduler)){
#if GODOT4 &&DEBUG
            throw new Exception($"[Miros.Connect] scheduler of {type} not found");
#else
            return;
#endif
        }
        var job = _jobProvider.GetJob(state);
        scheduler.RemoveJob(job);
    }

    public void Update(double delta)
    {
        foreach(var scheduler in _schedulers.Values){
            scheduler.Update(delta);
        }
    }

    public void PhysicsUpdate(double delta)
    {
        foreach(var scheduler in _schedulers.Values){
            scheduler.PhysicsUpdate(delta);
        }
    }


    public IJob[] GetAllJobs()
    {
        return _jobProvider.GetAllJobs();
    }

    public JobBase GetNowJob(Type stateType,Tag layer){
        if(!_schedulers.TryGetValue(stateType,out var scheduler)){
            return null;
        }
        return scheduler.GetNowJob(layer);
    }

    public JobBase GetLastJob(Type stateType,Tag layer){
        if(!_schedulers.TryGetValue(stateType,out var scheduler)){
            return null;
        }
        return scheduler.GetLastJob(layer);
    }

    public double GetCurrentJobTime(Type stateType,Tag layer){
        if(!_schedulers.TryGetValue(stateType,out var scheduler)){
            return 0;
        }
        return scheduler.GetCurrentJobTime(layer);
    }

}