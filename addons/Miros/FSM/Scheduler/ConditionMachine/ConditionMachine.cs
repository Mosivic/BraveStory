using System.Linq;
using FSM.Job;

namespace FSM.Scheduler;

public class ConditionMachine : AbsScheduler, IScheduler
{
    public void AddJob(IJob job)
    {
        var layer = job.Layer;
        if (!WaitingJobs.ContainsKey(layer))
        {
            WaitingJobs[layer] = [];
            RunningJobs[layer] = [];
        }

        var index = WaitingJobs[layer].FindIndex(j => job.Priority > j.Priority);
        WaitingJobs[layer].Insert(index + 1, job);
    }


    public void RemoveJob(IJob job)
    {
        var layer = job.Layer;
        if (WaitingJobs.ContainsKey(layer) && WaitingJobs[layer].Contains(job))
            WaitingJobs[layer].Remove(job);
    }

    public bool HasJobRunning(IJob job)
    {
        return RunningJobs[job.Layer].Contains(job);
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
                if (!job.CanEnter()) continue;

                var layerRunningJobsCount = RunningJobs[layer].Count;
                if (layerRunningJobsCount < layer.OnRunningJobMaxCount)
                {
                    PushRunningJob(layer, job);
                }
                else if (job.Priority > RunningJobs[layer].Last().Priority)
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


    private void PushRunningJob(Layer layer, IJob job)
    {
        WaitingJobs[layer].Remove(job);
        if (job.IsStack)
        {
            var index = RunningJobs[layer].FindIndex(j => j.Name == job.Name);
            if (index != -1)
            {
                RunningJobs[layer][index].Stack(job.Source);
                return;
            }
        }

        job.Enter();

        var highPriorityJob = RunningJobs[layer].FindIndex(j => j.Priority > job.Priority);
        RunningJobs[layer].Insert(highPriorityJob + 1, job);
    }


    private void PopRunningJob(Layer layer, IJob job)
    {
        RunningJobs[layer].Remove(job);
        var index = WaitingJobs[layer].FindIndex(j => job.Priority > j.Priority);
        WaitingJobs[layer].Insert(index + 1, job);

        job.Exit();
    }
}