using System.Collections.Generic;
using System.Linq;
using GPC.Job.Config;

namespace GPC.AI;

public class ConditionMachine: AbsScheduler
{
    protected Dictionary<string, State> JobsExecute = new();

    public ConditionMachine(List<State> states) : base(states)
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

    private State _GetBestJobCfg(string layer)
    {
        List<State> candicateJobsCfg = [];
        foreach (var cfg in States)
        {
            var jobLayer = cfg.Layer;
            if (jobLayer == layer && JobWrapper.CanExecute(cfg)) candicateJobsCfg.Add(cfg);
        }

        if (candicateJobsCfg.Count == 0)
            return null;
        return GetHighestCfg(candicateJobsCfg);
    }

    public static State GetHighestCfg(List<State> States)
    {
        if (States.Count == 0) return null;
        State bestState = null;
        foreach (var State in States)
        {
            if (bestState == null) bestState = State;
            if (State.Priority > bestState.Priority) bestState = State;
        }

        return bestState;
    }
}