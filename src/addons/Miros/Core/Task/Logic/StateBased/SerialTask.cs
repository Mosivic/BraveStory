/*
    串行任务，所有子任务都执行完毕后，任务成功
    注意⚠️：
    1. 因为结束条件固定为所有子任务顺序执行完毕，所以 ExitConditionFunc 无效
*/
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class SerialTaskWithStateBased<TState, THost, TContext, TExecuteArgs>(CompoundStateWithStateBased state) : Task<TState, THost, TContext, TExecuteArgs>()
    where TState : State, new()
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
{
    protected int CurrentIndex = 0;
    protected List<TaskBase> Tasks = [];



    protected override void OnAdd()
    {
        base.OnAdd();

        foreach (var subState in state.SubStates)
        {
            var subTask = TaskCreator.GetTask(subState);
            Tasks.Add(subTask);
        }

    }

    public override void Enter()
    {
        base.Enter();
        Tasks[CurrentIndex].Enter();
    }

    public override void Exit()
    {
        base.Exit();

        Tasks[CurrentIndex].Exit();
    }


    public override void Update(double delta)
    {
        base.Update(delta);

        if(CurrentIndex < Tasks.Count)
        {
            var subTask = Tasks[CurrentIndex];

            if(subTask.CanExit())
            {
                CurrentIndex++;
                subTask.Exit();
            }


            subTask.Update(delta);
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);

        if(CurrentIndex < Tasks.Count)
            Tasks[CurrentIndex].PhysicsUpdate(delta);
        
    }

    public override bool CanExit()
    {
        return CurrentIndex == Tasks.Count;
    }
}

