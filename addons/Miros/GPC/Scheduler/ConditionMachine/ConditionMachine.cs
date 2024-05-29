using System.Collections.Generic;
using GPC.Job.Config;

namespace GPC.AI;

public class ConditionMachine<T> : AbsScheduler<T> where T : class, IState
{
    protected Dictionary<string, T> JobsExecute = new();

    public ConditionMachine(StateSpace stateSpace) : base(stateSpace)
    {
        foreach (var state in States)
        {
            var layer = state.Layer;
            JobsExecute.TryAdd(layer, null);
        }
    }

    public override void Update(double delta)
    {
        _UpdateJob();
        foreach (var state in JobsExecute.Values)
            if (state != null)
                JobWrapper.Update(state, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var state in JobsExecute.Values)
            if (state != null)
                JobWrapper.PhysicsUpdate(state, delta);
    }

    private void _UpdateJob()
    {
        foreach (var layer in JobsExecute.Keys)
        {
            var currentState = JobsExecute[layer];
            var nextState = _GetBestState(layer);

            if (currentState == null)
            {
                if (nextState == null) return;

                JobsExecute[layer] = nextState;
                JobWrapper.Enter(nextState);
            }
            else
            {
                if (currentState.Status != Status.Running)
                {
                    if (nextState == null)
                    {
                        JobWrapper.Exit(currentState);
                        JobsExecute[layer] = null;
                    }
                    else
                    {
                        JobWrapper.Exit(currentState);
                        JobsExecute[layer] = nextState;
                        JobWrapper.Enter(nextState);
                    }
                }
                else
                {
                    if (nextState == null) return;

                    if (nextState.Priority > currentState.Priority)
                    {
                        JobWrapper.Exit(currentState);
                        JobsExecute[layer] = nextState;
                        JobWrapper.Enter(nextState);
                    }
                }
            }
        }
    }

    private T _GetBestState(string layer)
    {
        List<T> candicateJobsCfg = [];
        foreach (var state in States)
        {
            var jobLayer = state.Layer;
            if (jobLayer == layer && JobWrapper.CanExecute(state)) candicateJobsCfg.Add(state);
        }

        if (candicateJobsCfg.Count == 0)
            return null;
        return GetHighestCfg(candicateJobsCfg);
    }

    private static T GetHighestCfg(List<T> states)
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