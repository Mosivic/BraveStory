
using System.Linq;
using Godot;

namespace Miros.Core;

public class RandomTask: TaskBase<CompoundState>
{
    // 各子任务权重，用于随机选择子任务
    protected virtual float[] RandomWeights { get; set;} = [];

    // 当前选择的子任务索引
    protected int CurrentIndex = 0;

    protected TaskBase<State> CurrentTask = null;

    public override void Enter(State state)
    {
        base.Enter(state);
        var compoundState = state as CompoundState;
        CurrentIndex = SelectTaskBasedOnWeights(compoundState);
        CurrentTask = TaskProvider.GetTask(compoundState.SubStates[CurrentIndex].TaskType) as TaskBase<State>;
        
        if(IsCurrentCanEnter(state))
            CurrentTask.Enter(state);
        else
            state.Status = RunningStatus.Failed;
    }


    public override void Exit(State state)
    {   
        base.Exit(state);
        CurrentTask.Exit(state);
    }


    public override void Update(State state, double delta)
    {
        base.Update(state, delta);
        CurrentTask.Update(state, delta);
    }


    public override void PhysicsUpdate(State state, double delta)
    {
        base.PhysicsUpdate(state, delta);
        CurrentTask.PhysicsUpdate(state, delta);
    }


    protected bool IsCurrentCanEnter(State state)
    {
        return CurrentTask.CanEnter(state);
    }

    protected int SelectTaskBasedOnWeights(State state)
    {
        var compoundState = state as CompoundState;
        if (RandomWeights == null || RandomWeights.Length == 0)
        {
            return 0; // 默认选择第一个子任务
        }

        float totalWeight = RandomWeights.Sum();
        float randomValue = GD.Randf() * totalWeight;
        float cumulativeWeight = 0;

        int RandomWeightsLength = RandomWeights.Length;

        for (int i = 0; i < compoundState.SubStates.Length; i++)
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