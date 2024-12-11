using System.Collections.Generic;
using System.Linq;

namespace Miros.Core;

public class ConditionMachine : ExecutorBase<TaskBase>
{
    protected readonly Dictionary<Tag, List<TaskBase>> RunningTasks = new();
    protected Dictionary<Tag, List<TaskBase>> WaitingTasks { get; set; } = new();

    public override void AddTask(ITask task,StateExecuteArgs args=null)
    {
        var conditionTask = task as TaskBase;
        var layer = conditionTask.Tag;

        if (!WaitingTasks.ContainsKey(layer))
        {
            WaitingTasks[layer] = [];
            RunningTasks[layer] = [];
        }

        var index = WaitingTasks[layer].FindIndex(j => conditionTask.Priority > j.Priority);
        WaitingTasks[layer].Insert(index + 1, conditionTask);
    }


    public override void RemoveTask(ITask task)
    {
        var conditionTask = task as TaskBase;
        var layer = conditionTask.Tag;
        if (WaitingTasks.ContainsKey(layer) && WaitingTasks[layer].Contains(conditionTask))
            WaitingTasks[layer].Remove(conditionTask);
    }

    public override bool HasTaskRunning(ITask task)
    {
        var conditionTask = task as TaskBase;
        return RunningTasks[conditionTask.Tag].Contains(conditionTask);
    }


    public override void Update(double delta)
    {
        WaitingTasksToRunningTasks();

        foreach (var layer in RunningTasks.Keys)
            for (var i = 0; i < RunningTasks[layer].Count; i++)
            {
                var task = RunningTasks[layer][i];
                if (task.CanExit())
                    PopRunningTask(layer, task);
                else
                    task.Update(delta);
            }
    }


    public override void PhysicsUpdate(double delta)
    {
        foreach (var layer in RunningTasks.Keys)
            for (var i = 0; i < RunningTasks[layer].Count; i++)
                RunningTasks[layer][i].PhysicsUpdate(delta);
    }


    private void WaitingTasksToRunningTasks()
    {
        foreach (var layer in WaitingTasks.Keys)
            for (var i = WaitingTasks[layer].Count - 1; i >= 0; i--)
            {
                var task = WaitingTasks[layer][i];

                if (!task.CanEnter())
                    continue;


                var layerRunningTasksCount = RunningTasks[layer].Count;
                if (layerRunningTasksCount < 3) //限定最大并行数
                {
                    PushRunningTask(layer, task);
                }
                else if (task.Priority > RunningTasks[layer].Last().Priority)
                {
                    PopRunningTask(layer, RunningTasks[layer].Last());
                    PushRunningTask(layer, task);
                }
                else
                {
                    break;
                }
            }
    }


    private void PushRunningTask(Tag layer, TaskBase task)
    {
        WaitingTasks[layer].Remove(task);
        // if (task.State.IsStack)
        // {
        // 	var index = RunningTasks[layer].FindIndex(j => j.State.Name == task.State.Name);
        // 	if (index != -1)
        // 	{
        // 		RunningTasks[layer][index].Stack(task.State.Source);
        // 		return;
        // 	}
        // }

        task.Enter();

        var highPriorityTask = RunningTasks[layer].FindIndex(j => j.Priority > task.Priority);
        RunningTasks[layer].Insert(highPriorityTask + 1, task);
    }


    private void PopRunningTask(Tag layer, TaskBase task)
    {
        RunningTasks[layer].Remove(task);
        var index = WaitingTasks[layer].FindIndex(j => task.Priority > j.Priority);
        WaitingTasks[layer].Insert(index + 1, task);

        task.Exit();
    }
}