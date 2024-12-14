/*
    串行任务，所有子任务都执行完毕后，任务成功
    注意⚠️：
    1. 因为结束条件固定为所有子任务顺序执行完毕，所以 ExitConditionFunc 无效
*/
using Godot;

namespace Miros.Core;

public class SerialTask<TState, THost, TContext, TExecuteArgs>(CompoundState state) : Task<TState, THost, TContext, TExecuteArgs>()
    where TState : State, new()
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
{
    protected int CurrentIndex = 0;
    protected Task<TState, THost, TContext, TExecuteArgs> CurrentTask;



    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        CurrentTask.Exit();
    }


    public override void Update(double delta)
    {
        base.Update(delta);

        if(CurrentIndex < state.SubTaskTypes.Length)
        {
            var subTaskType = state.SubTaskTypes[CurrentIndex];

            if(CurrentTask == null)
            {
                CurrentTask = TaskCreator.GetTask<TState, THost, TContext, TExecuteArgs>(subTaskType);
                CurrentTask.Enter();
            }

            if(CurrentTask.CanExit())
            {
                CurrentIndex++;
                CurrentTask.Exit();
            }


            CurrentTask.Update(delta);
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);

        if(CurrentIndex < state.SubTaskTypes.Length)
            CurrentTask.PhysicsUpdate(delta);
        
    }

    public override bool CanExit()
    {
        return CurrentIndex == state.SubTaskTypes.Length;
    }
}

