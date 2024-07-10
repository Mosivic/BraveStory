﻿using System.Collections.Generic;
using System.Linq;
using GPC.Scheduler;
using GPC.States;

namespace GPC.Job.Executor;

public class Connect<TJobProvider, TScheduler> :IConnect<TJobProvider,TScheduler>  
    where TJobProvider:IJobProvider,new()
    where TScheduler : IScheduler,new()
{
    protected TJobProvider JobProvider { get; set; } 
    protected TScheduler Scheduler { get; set; }
    
    public Connect(List<AbsState> states)
    {
        JobProvider = new TJobProvider();
        Scheduler = new TScheduler();
        
        foreach (var job in states.Select(state => JobProvider.GetJob(state)))
        {
            Scheduler.AddJob(job);
        }

    }
    
    
    public void AddState(AbsState state)
    {
        var job = JobProvider.GetJob(state);
        Scheduler.AddJob(job);
    }

    
    public void RemoveState(AbsState state)
    {
        var job = JobProvider.GetJob(state);
        Scheduler.RemoveJob(job);
    }

    public void Update(double delta)
    {
        Scheduler.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        Scheduler.PhysicsUpdate(delta);
    }
}