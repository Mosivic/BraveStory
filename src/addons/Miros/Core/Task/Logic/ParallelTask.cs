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

public class ParallelTask: TaskBase<State>
{
    protected List<TaskBase<State>> SubTasks = [];


    protected override void OnAdd(State state)
    {
        base.OnAdd(state);

        foreach (var subState in state.SubStates)
        {
            var subTask = TaskProvider.GetTask(subState.TaskType) as TaskBase<State>;
            SubTasks.Add(subTask);
        }
    }

    public override void Enter(State state)
    {
        base.Enter(state);
        foreach (var subTask in SubTasks)
            subTask.Enter(state);
    }


    public override void Exit(State state)
    {
        base.Exit(state);
        foreach (var subTask in SubTasks)
            subTask.Exit(state);
        
    }

    public override void Update(State state, double delta)
    {
        base.Update(state, delta);
        foreach (var subTask in SubTasks)
            subTask.Update(state, delta);
            
    }

    public override void PhysicsUpdate(State state, double delta)
    {
        base.PhysicsUpdate(state, delta);
        foreach (var subTask in SubTasks)
            subTask.PhysicsUpdate(state, delta);
    }


    public override bool CanExit(State state) 
    {
        return SubTasks.All(subTask => subTask.CanExit(state));
    }



}