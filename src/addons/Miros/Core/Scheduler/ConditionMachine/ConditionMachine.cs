using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class ConditionMachine : SchedulerBase<JobBase>
{
	protected readonly Dictionary<Tag, List<JobBase>> RunningJobs = new();
	protected Dictionary<Tag, List<JobBase>> WaitingJobs { get; set; } = new();

	public override void AddJob(JobBase job)
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


	public override void RemoveJob(JobBase job)
	{
		var layer = job.Layer;
		if (WaitingJobs.ContainsKey(layer) && WaitingJobs[layer].Contains(job))
			WaitingJobs[layer].Remove(job);
	}

	public override  bool HasJobRunning(JobBase job)
	{
		return RunningJobs[job.Layer].Contains(job);
	}


	public override void Update(double delta)
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


	public override void PhysicsUpdate(double delta)
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


	private void PushRunningJob(Tag layer, JobBase job)
	{
		WaitingJobs[layer].Remove(job);
		// if (job.State.IsStack)
		// {
		// 	var index = RunningJobs[layer].FindIndex(j => j.State.Name == job.State.Name);
		// 	if (index != -1)
		// 	{
		// 		RunningJobs[layer][index].Stack(job.State.Source);
		// 		return;
		// 	}
		// }

		job.Enter();

		var highPriorityJob = RunningJobs[layer].FindIndex(j => j.Priority > job.Priority);
		RunningJobs[layer].Insert(highPriorityJob + 1, job);
	}


	private void PopRunningJob(Tag layer, JobBase job)
	{
		RunningJobs[layer].Remove(job);
		var index = WaitingJobs[layer].FindIndex(j => job.Priority > j.Priority);
		WaitingJobs[layer].Insert(index + 1, job);

		job.Exit();
	}
}
