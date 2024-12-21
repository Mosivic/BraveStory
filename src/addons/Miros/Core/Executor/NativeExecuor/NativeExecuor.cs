using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class ConditionMachine : ExecutorBase, IExecutor
{
    protected readonly Dictionary<Tag, List<State>> RunningStates = [];
    protected Dictionary<Tag, List<State>> WaitingStates = [];

    public override void AddState(State state)
    {
        var layer = state.Tag;

        if (!WaitingStates.ContainsKey(layer))
        {
            WaitingStates[layer] = [];
            RunningStates[layer] = [];
        }

        var index = WaitingStates[layer].FindIndex(j => state.Priority > j.Priority);
        WaitingStates[layer].Insert(index + 1, state);
    }


    public override void RemoveState(State state)
    {
        var layer = state.Tag;
        if (WaitingStates.ContainsKey(layer) && WaitingStates[layer].Contains(state))
            WaitingStates[layer].Remove(state);
    }

    public override bool HasStateRunning(State state)
    {
        return RunningStates[state.Tag].Contains(state);
    }


    public override void Update(double delta)
    {
        WaitingTasksToRunningTasks();

        foreach (var layer in RunningStates.Keys)
            for (var i = 0; i < RunningStates[layer].Count; i++)
            {
                var state = RunningStates[layer][i];
                if (state.Task.CanExit(state))
                    PopRunningTask(layer, state);
                else
                    state.Task.Update(state, delta);
            }
    }


    public override void PhysicsUpdate(double delta)
    {
        foreach (var layer in RunningStates.Keys)
            for (var i = 0; i < RunningStates[layer].Count; i++)
                RunningStates[layer][i].Task.PhysicsUpdate(RunningStates[layer][i], delta);
    }


    private void WaitingTasksToRunningTasks()
    {
        foreach (var layer in WaitingStates.Keys)
            for (var i = WaitingStates[layer].Count - 1; i >= 0; i--)
            {
                var state = WaitingStates[layer][i];

                if (!state.Task.CanEnter(state))
                    continue;


                var layerRunningStatesCount = RunningStates[layer].Count;
                if (layerRunningStatesCount < 3) //限定最大并行数
                {
                    PushRunningTask(layer, state);
                }
                else if (state.Priority > RunningStates[layer].Last().Priority)
                {
                    PopRunningTask(layer, RunningStates[layer].Last());
                    PushRunningTask(layer, state);
                }
                else
                {
                    break;
                }
            }
    }


    private void PushRunningTask(Tag layer, State state)
    {
        WaitingStates[layer].Remove(state);
        // if (task.State.IsStack)
        // {
        // 	var index = RunningTasks[layer].FindIndex(j => j.State.Name == task.State.Name);
        // 	if (index != -1)
        // 	{
        // 		RunningTasks[layer][index].Stack(task.State.Source);
        // 		return;
        // 	}
        // }

        state.Task.Enter(state);

        var highPriorityTask = RunningStates[layer].FindIndex(j => j.Priority > state.Priority);
        RunningStates[layer].Insert(highPriorityTask + 1, state);
    }


    private void PopRunningTask(Tag layer, State state)
    {
        RunningStates[layer].Remove(state);
        var index = WaitingStates[layer].FindIndex(j => state.Priority > j.Priority);
        WaitingStates[layer].Insert(index + 1, state);

        state.Task.Exit(state);
    }
}