using System;
using System.Collections.Generic;
using System.Linq;
using FSM.Job;
using FSM.States;

namespace FSM.Scheduler;

public class ConditionMachine : AbsScheduler, IScheduler
{
    protected readonly Dictionary<GameplayTag, List<IJob>> RunningJobs = new();
    protected Dictionary<GameplayTag, List<IJob>> WaitingJobs { get; set; } = new();

    public void AddJob(IJob job)
    {
        var layer = job.State.Tag;

        if (!WaitingJobs.ContainsKey(layer))
        {
            WaitingJobs[layer] = [];
            RunningJobs[layer] = [];
        }

        var index = WaitingJobs[layer].FindIndex(j => job.State.Priority > j.State.Priority);
        WaitingJobs[layer].Insert(index + 1, job);
    }


    public void RemoveJob(IJob job)
    {
        var layer = job.State.Tag;
        if (WaitingJobs.ContainsKey(layer) && WaitingJobs[layer].Contains(job))
            WaitingJobs[layer].Remove(job);
    }

    public bool HasJobRunning(IJob job)
    {
        return RunningJobs[job.State.Tag].Contains(job);
    }


    public void Update(double delta)
    {
        WaitingJobsToRunningJobs();

        foreach (var layer in RunningJobs.Keys)
            for (var i = 0; i < RunningJobs[layer].Count; i++)
            {
                var job = RunningJobs[layer][i];
                if (job.CanExit())
                    PopRunningJob(layer, job);
                else
                    job.Update(delta);
            }
    }


    public void PhysicsUpdate(double delta)
    {
        foreach (var layer in RunningJobs.Keys)
            for (var i = 0; i < RunningJobs[layer].Count; i++)
                RunningJobs[layer][i].PhysicsUpdate(delta);
    }


    private void WaitingJobsToRunningJobs()
    {
        foreach (var layer in WaitingJobs.Keys)
            for (var i = WaitingJobs[layer].Count - 1; i >= 0; i--)
            {
                var job = WaitingJobs[layer][i];
                
                if (!job.CanEnter()) 
                    continue;
                

                var layerRunningJobsCount = RunningJobs[layer].Count;
                if (layerRunningJobsCount < 3) //限定最大并行数
                {
                    PushRunningJob(layer, job);
                }
                else if (job.State.Priority > RunningJobs[layer].Last().State.Priority)
                {
                    PopRunningJob(layer, RunningJobs[layer].Last());
                    PushRunningJob(layer, job);
                }
                else
                {
                    break;
                }
            }
    }


    private void PushRunningJob(GameplayTag layer, IJob job)
    {
        WaitingJobs[layer].Remove(job);
        if (job.State.IsStack)
        {
            var index = RunningJobs[layer].FindIndex(j => j.State.Name == job.State.Name);
            if (index != -1)
            {
                RunningJobs[layer][index].Stack(job.State.Source);
                return;
            }
        }

        job.Enter();

        var highPriorityJob = RunningJobs[layer].FindIndex(j => j.State.Priority > job.State.Priority);
        RunningJobs[layer].Insert(highPriorityJob + 1, job);
    }


    private void PopRunningJob(GameplayTag layer, IJob job)
    {
        RunningJobs[layer].Remove(job);
        var index = WaitingJobs[layer].FindIndex(j => job.State.Priority > j.State.Priority);
        WaitingJobs[layer].Insert(index + 1, job);

        job.Exit();
    }

    public AbsState GetNowState(GameplayTag Layer)
    {
        throw new NotImplementedException();
    }

    public AbsState GetLastState(GameplayTag Layer)
    {
        throw new NotImplementedException();
    }

}