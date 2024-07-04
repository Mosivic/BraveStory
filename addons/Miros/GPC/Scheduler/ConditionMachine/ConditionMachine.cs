using System.Collections.Generic;
using Godot;
using GPC.States;

namespace GPC.Scheduler;

public class ConditionMachine : AbsScheduler
{
    private Dictionary<string, State> _jobsExecute = new();

    public ConditionMachine(StateSet stateSet) : base(stateSet)
    {
        foreach (var state in stateSet.States)
        {
            var layer = state.Layer;
            _jobsExecute.TryAdd(layer, null);
        }
    }

    public override void Update(double delta)
    {
        _UpdateJob();
        foreach (var state in _jobsExecute.Values)
            if (state != null)
                JobWrapper.Update(state, delta);
    }

    public override void PhysicsUpdate(double delta)
    {
        foreach (var state in _jobsExecute.Values)
            if (state != null)
                JobWrapper.PhysicsUpdate(state, delta);
    }

    private void _UpdateJob()
    {
        foreach (var layer in _jobsExecute.Keys)
        {
            var currentState = _jobsExecute[layer];
            var nextState = _GetBestState(layer);

            if (currentState == null)
            {
                if (nextState == null) return;

                _jobsExecute[layer] = nextState;
                JobWrapper.Enter(nextState);
            }
            else
            {
                if (currentState.Status != Status.Running)
                {
                    if (nextState == null)
                    {
                        JobWrapper.Exit(currentState);
                        _jobsExecute[layer] = null;
                    }
                    else
                    {
                        JobWrapper.Exit(currentState);
                        _jobsExecute[layer] = nextState;
                        JobWrapper.Enter(nextState);
                    }
                }
                else
                {
                    if (nextState == null) return;

                    if (nextState.Priority > currentState.Priority)
                    {
                        JobWrapper.Exit(currentState);
                        _jobsExecute[layer] = nextState;
                        JobWrapper.Enter(nextState);
                    }
                }
            }
        }
    }

    private State _GetBestState(string layer)
    {
        List<State> candicateJobsCfg = [];
        foreach (var state in StateSet.States)
        {
            var jobLayer = state.Layer;
            if (jobLayer == layer && JobWrapper.CanExecute((State)state)) candicateJobsCfg.Add((State)state);
        }

        if (candicateJobsCfg.Count == 0)
            return null;
        return GetHighestCfg(candicateJobsCfg);
    }


    private static State GetHighestCfg(List<State> states)
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