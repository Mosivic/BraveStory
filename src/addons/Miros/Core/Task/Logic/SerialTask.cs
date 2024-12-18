/*
    串行任务，所有子任务都执行完毕后，任务成功
    注意⚠️：
    1. 因为结束条件固定为所有子任务顺序执行完毕，所以 ExitConditionFunc 无效
*/
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class SerialTask: TaskBase<State>
{
    protected State CurrentState = null;
    protected int CurrentIndex = 0;
    protected int SubStatesCount = 0;

    public override void TriggerOnAdd(State state)
    {
        base.TriggerOnAdd(state);

        foreach (var subState in state.SubStates)
        {
            var subTask = TaskProvider.GetTask(subState.TaskType);
            state.Task = subTask;
        }

        SubStatesCount = state.SubStates.Length;
        if(SubStatesCount > 0)
            CurrentState = state.SubStates[0];
    }

    public override void Enter(State state)
    {
        base.Enter(state);
        CurrentState.Task.Enter(state);
    }

    public override void Exit(State state)
    {
        base.Exit(state);

        CurrentState.Task.Exit(state);
    }


    public override void Update(State state, double delta)
    {
        base.Update(state, delta);

        if(CurrentIndex < SubStatesCount)
        {

            if(CurrentState.Task.CanExit(state))
            {
                CurrentIndex++;
                CurrentState = state.SubStates[CurrentIndex];
                CurrentState.Task.Exit(state);
            }


            CurrentState.Task.Update(state, delta);
        }
    }

    public override void PhysicsUpdate(State state, double delta)
    {
        base.PhysicsUpdate(state, delta);

        if(CurrentIndex < SubStatesCount)
            CurrentState.Task.PhysicsUpdate(state, delta);
        
    }

    public override bool CanExit(State state)
    {
        return CurrentIndex == SubStatesCount || SubStatesCount == 0;
    }
}

