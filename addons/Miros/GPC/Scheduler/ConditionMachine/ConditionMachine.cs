using System.Collections.Generic;
using System.Linq;
using GPC.Job.Config;

namespace GPC.AI;

public class ConditionMachine : AbsScheduler
{
    protected Dictionary<string, State> _JobsExecute = new();

    public ConditionMachine(List<State> states) : base(states)
    {
        foreach (var cfg in States)
        {
            var layer = cfg.Layer;
            _JobsExecute.TryAdd(layer, null);
        }
    }

    public override void Update(double delta)
    {
        _UpdateJob();
        foreach (var cfg in _JobsExecute.Values)
            if (cfg != null)
                JobWrapper.Update(cfg, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var cfg in _JobsExecute.Values)
            if (cfg != null)
                JobWrapper.PhysicsUpdate(cfg, delta);
    }

    private void _UpdateJob()
    {
        foreach (var layer in _JobsExecute.Keys)
        {
            var currentJobCfg = _JobsExecute[layer];
            var nextJobCfg = _GetBestJobCfg(layer);

            if (currentJobCfg == null)
            {
                if (nextJobCfg == null) return;

                _JobsExecute[layer] = nextJobCfg;
                JobWrapper.Enter(nextJobCfg);
            }
            else
            {
                if (currentJobCfg.Status != Status.Running)
                {
                    if (nextJobCfg == null)
                    {
                        JobWrapper.Exit(currentJobCfg);
                        _JobsExecute[layer] = null;
                    }
                    else
                    {
                        JobWrapper.Exit(currentJobCfg);
                        _JobsExecute[layer] = nextJobCfg;
                        JobWrapper.Enter(nextJobCfg);
                    }
                }
                else
                {
                    if (nextJobCfg == null) return;

                    if (nextJobCfg.Priority > currentJobCfg.Priority)
                    {
                        JobWrapper.Exit(currentJobCfg);
                        _JobsExecute[layer] = nextJobCfg;
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

    public static State GetHighestCfg(List<State> states)
    {
        if (states.Count == 0) return null;
        State bestState = null;
        foreach (var state in states)
        {
            if (bestState == null) bestState = state;
            if (state.Priority > bestState.Priority) bestState = state;
        }

        return bestState;
    }
}