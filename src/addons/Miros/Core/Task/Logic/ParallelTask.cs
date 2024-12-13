/*
    并行任务，所有子任务都执行完毕后，任务成功
    如果有一个子任务失败，则任务失败
    基于 CompoundState 

    注意⚠️：
    1. 因为结束条件固定为所有子任务运行完毕，所以 ExitConditionFunc 无效

*/
using System.Linq;
using Godot;

namespace Miros.Core;

public class ParallelTask<TState, THost, TContext, TExecuteArgs>(CompoundState state) : Task<TState, THost, TContext, TExecuteArgs>()
    where TState : State, new()
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
{
    public override void Enter()
    {
        base.Enter();
        foreach (var subState in state.SubStates)
        {
            subState.OwnerTask = TaskCreator.GetTask(subState);
            subState.OwnerTask.Enter();
        }
    }

    public override void Exit()
    {
        base.Exit();
        foreach (var subState in state.SubStates)
            subState.OwnerTask.Exit();
        
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        foreach (var subState in state.SubStates)
            subState.OwnerTask.Update(delta);
        
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);
        foreach (var subState in state.SubStates)
            subState.OwnerTask.PhysicsUpdate(delta);
    }


    public override bool CanExit() 
    {
        return state.SubStates.All(subState => subState.OwnerTask.CanExit());
    }

}


