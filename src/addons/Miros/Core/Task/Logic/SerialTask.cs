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

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        var subState = state.SubStates[CurrentIndex];  
        subState.OwnerTask.Exit();
    }


    public override void Update(double delta)
    {
        base.Update(delta);

        if(CurrentIndex < state.SubStates.Count)
        {
            var subState = state.SubStates[CurrentIndex];

            if(subState.OwnerTask == null)
            {
                subState.OwnerTask = TaskCreator.GetTask(subState);
                subState.OwnerTask.Enter();
            }

            if(subState.OwnerTask.CanExit())
            {
                CurrentIndex++;
                subState.OwnerTask.Exit();
            }


            subState.OwnerTask.Update(delta);
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);

        if(CurrentIndex < state.SubStates.Count)
        {
            var subState = state.SubStates[CurrentIndex];
            subState.OwnerTask.PhysicsUpdate(delta);
        }
    }

    public override bool CanExit()
    {
        return CurrentIndex == state.SubStates.Count;
    }
}

