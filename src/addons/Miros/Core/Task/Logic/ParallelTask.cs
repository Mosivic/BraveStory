/*
    并行任务，所有子任务都执行完毕后，任务成功
    如果有一个子任务失败，则任务失败
    基于 CompoundState 

    注意⚠️：
    1. 因为结束条件固定为所有子任务运行完毕，所以 ExitConditionFunc 无效

*/
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Miros.Core;

public class ParallelTask<TState, THost, TContext, TExecuteArgs>(CompoundState state) : Task<TState, THost, TContext, TExecuteArgs>()
    where TState : State, new()
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
{
    protected List<Task<TState, THost, TContext, TExecuteArgs>> SubTasks = [];


    protected override void OnAdd()
    {
        base.OnAdd();
        foreach (var subTaskType in state.SubTaskTypes)
        {
            var subTask = TaskCreator.GetTask<TState, THost, TContext, TExecuteArgs>(subTaskType);
            SubTasks.Add(subTask);
        }
    }

    public override void Enter()
    {
        base.Enter();
        foreach (var subTask in SubTasks)
            subTask.Enter();
    }


    public override void Exit()
    {
        base.Exit();
        foreach (var subTask in SubTasks)
            subTask.Exit();
        
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        foreach (var subTask in SubTasks)
            subTask.Update(delta);
        
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);
        foreach (var subTask in SubTasks)
            subTask.PhysicsUpdate(delta);
    }


    public override bool CanExit() 
    {
        return SubTasks.All(subTask => subTask.CanExit());
    }



}