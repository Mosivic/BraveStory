/*
    串行任务，所有子任务都执行完毕后，任务成功
    注意⚠️：
    1. 因为结束条件固定为所有子任务顺序执行完毕，所以 ExitConditionFunc 无效
*/
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class SerialTask: TaskBase<CompoundState>
{
    protected int CurrentIndex = 0;
    protected List<TaskBase<State>> Tasks = [];


    public override void TriggerOnAdd(State state)
    {
        base.TriggerOnAdd(state);
        var compoundState = state as CompoundState;
        foreach (var subState in compoundState.SubStates)
        {
            var subTask = TaskProvider.GetTask(subState.TaskType) as TaskBase<State>;
            Tasks.Add(subTask);
        }

    }

    public override void Enter(State state)
    {
        base.Enter(state);
        Tasks[CurrentIndex].Enter(state);
    }

    public override void Exit(State state)
    {
        base.Exit(state);

        Tasks[CurrentIndex].Exit(state);
    }


    public override void Update(State state, double delta)
    {
        base.Update(state, delta);

        if(CurrentIndex < Tasks.Count)
        {
            var subTask = Tasks[CurrentIndex];

            if(subTask.CanExit(state))
            {
                CurrentIndex++;
                subTask.Exit(state);
            }


            subTask.Update(state, delta);
        }
    }

    public override void PhysicsUpdate(State state, double delta)
    {
        base.PhysicsUpdate(state, delta);

        if(CurrentIndex < Tasks.Count)
            Tasks[CurrentIndex].PhysicsUpdate(state, delta);
        
    }

    public override bool CanExit(State state)
    {
        return CurrentIndex == Tasks.Count;
    }
}

