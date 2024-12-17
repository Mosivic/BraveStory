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



    public override void TriggerOnAdd(CompoundState state)
    {
        base.TriggerOnAdd(state);

        foreach (var subState in state.SubStates)
        {
            var subTask = TaskProvider.GetTask(subState);
            Tasks.Add(subTask);
        }

    }

    public override void Enter(CompoundState state)
    {
        base.Enter(state);
        Tasks[CurrentIndex].Enter(state);
    }

    public override void Exit(CompoundState state)
    {
        base.Exit(state);

        Tasks[CurrentIndex].Exit(state);
    }


    public override void Update(CompoundState state, double delta)
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

    public override void PhysicsUpdate(CompoundState state, double delta)
    {
        base.PhysicsUpdate(state, delta);

        if(CurrentIndex < Tasks.Count)
            Tasks[CurrentIndex].PhysicsUpdate(state, delta);
        
    }

    public override bool CanExit(CompoundState state)
    {
        return CurrentIndex == Tasks.Count;
    }
}

