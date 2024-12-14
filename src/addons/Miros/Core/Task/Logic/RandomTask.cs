
using System.Linq;
using Godot;

namespace Miros.Core;

public class RandomTask<TState, THost, TContext, TExecuteArgs>(CompoundState state) : Task<TState, THost, TContext, TExecuteArgs>()
    where TState : State, new()
    where THost : Node
    where TContext : Context
    where TExecuteArgs : ExecuteArgs
{
    // 各子任务权重，用于随机选择子任务
    protected virtual float[] RandomWeights { get; set;} = [];

    // 当前选择的子任务索引
    protected int CurrentIndex = 0;

    protected TaskBase CurrentTask = null;

    public override void Enter()
    {
        base.Enter();
        CurrentIndex = SelectTaskBasedOnWeights();
        CurrentTask = TaskCreator.GetTask<TState, THost, TContext, TExecuteArgs>(state.SubTaskTypes[CurrentIndex]);
        
        if(IsCurrentCanEnter())
            CurrentTask.Enter();
        else
            state.Status = RunningStatus.Failed;
    }


    public override void Exit()
    {   
        base.Exit();
        CurrentTask.Exit();
    }


    public override void Update(double delta)
    {
        base.Update(delta);
        CurrentTask.Update(delta);
    }


    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);
        CurrentTask.PhysicsUpdate(delta);
    }


    protected bool IsCurrentCanEnter()
    {
        return CurrentTask.CanEnter();
    }

    protected int SelectTaskBasedOnWeights()
    {
        if (RandomWeights == null || RandomWeights.Length == 0)
        {
            return 0; // 默认选择第一个子任务
        }

        float totalWeight = RandomWeights.Sum();
        float randomValue = GD.Randf() * totalWeight;
        float cumulativeWeight = 0;

        int RandomWeightsLength = RandomWeights.Length;

        for (int i = 0; i < state.SubTaskTypes.Length; i++)
        {
            if(i >= RandomWeightsLength) // 如果权重数组长度小于子任务数量，则每个子任务权重为1
                cumulativeWeight += 1;
            else
                cumulativeWeight += RandomWeights[i];

            if (randomValue < cumulativeWeight)
            {
                return i; 
            }
        }
        return 0; 
    }
}