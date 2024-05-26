using System.Collections.Generic;
using GPC.Job.Config;

namespace GPC.AI;

public class ConditionMachine<T> : AbsScheduler<T> where T : class, IState
{
    protected Dictionary<string, T> JobsExecute = new();

    public ConditionMachine(List<T> states, ConditionLib lib) : base(states, lib)
    {
        foreach (var cfg in States)
        {
            var layer = cfg.Layer;
            JobsExecute.TryAdd(layer, null);
        }
    }

    public override void Update(double delta)
    {
        _UpdateJob();
        foreach (var cfg in JobsExecute.Values)
            if (cfg != null)
                JobWrapper.Update(cfg, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var cfg in JobsExecute.Values)
            if (cfg != null)
                JobWrapper.PhysicsUpdate(cfg, delta);
    }

    private void _UpdateJob()
    {
        foreach (var layer in JobsExecute.Keys)
        {
            var currentJobCfg = JobsExecute[layer];
            var nextJobCfg = _GetBestJobCfg(layer);

            if (currentJobCfg == null)
            {
                if (nextJobCfg == null) return;

                JobsExecute[layer] = nextJobCfg;
                JobWrapper.Enter(nextJobCfg);
            }
            else
            {
                if (currentJobCfg.Status != Status.Running)
                {
                    if (nextJobCfg == null)
                    {
                        JobWrapper.Exit(currentJobCfg);
                        JobsExecute[layer] = null;
                    }
                    else
                    {
                        JobWrapper.Exit(currentJobCfg);
                        JobsExecute[layer] = nextJobCfg;
                        JobWrapper.Enter(nextJobCfg);
                    }
                }
                else
                {
                    if (nextJobCfg == null) return;

                    if (nextJobCfg.Priority > currentJobCfg.Priority)
                    {
                        JobWrapper.Exit(currentJobCfg);
                        JobsExecute[layer] = nextJobCfg;
                        JobWrapper.Enter(nextJobCfg);
                    }
                }
            }
        }
    }

    private T _GetBestJobCfg(string layer)
    {
        List<T> candicateJobsCfg = [];
        foreach (var cfg in States)
        {
            var jobLayer = cfg.Layer;
            if (jobLayer == layer && JobWrapper.CanExecute(cfg)) candicateJobsCfg.Add(cfg);
        }

        if (candicateJobsCfg.Count == 0)
            return null;
        return GetHighestCfg(candicateJobsCfg);
    }

    public static T GetHighestCfg(List<T> states)
    {
        if (states.Count == 0) return null;
        T bestState = null;
        foreach (var state in states)
        {
            if (bestState == null) bestState = state;
            if (state.Priority > bestState.Priority) bestState = state;
        }

        return bestState;
    }
}