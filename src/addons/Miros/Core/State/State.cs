using System;
using System.Collections.Generic;

namespace Miros.Core;

public class State
{
    // Core
    public string Name { get; init; }
    public Tag Sign { get; init; }
    public string Description { get; init; }
    
    public Type JobType { get; init; } = typeof(JobBase);
    public int Priority { get; init; } = 0;

    public Persona Owner { get; protected set; }
    public Persona Source { get; protected set; }
    public RunningStatus Status { get; set; } = RunningStatus.NoRun;
    public bool IsActive => Status == RunningStatus.Running;

    public double RunningTime { get; set; } = 0;

    public Dictionary<Type, StateComponent<JobBase>> Components { get; set; } = [];

    // 添加构造函数
    public State(string name, Tag sign)
    {
        Name = name;
        Sign = sign;
    }

    // 添加便捷的组件添加方法
    public State AddComponent<T>(Action<T> setup = null) where T : StateComponent<JobBase>, new()
    {
        var component = new T();
        setup?.Invoke(component);
        Components[typeof(T)] = component;
        return this;
    }

    // 特别为 StandardDelegateComponent 添加快捷方法
    // 扩展现有的 OnEnter 方法族
    public State OnEnter(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.EnterFunc = action);
    }

    public State OnEnterIf(Func<State, bool> condition)
    {
        return AddOrUpdateDelegate(c => c.EnterCondition = condition);
    }

    // 退出相关
    public State OnExit(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.ExitFunc = action);
    }

    public State OnExitIf(Func<State, bool> condition)
    {
        return AddOrUpdateDelegate(c => c.ExitCondition = condition);
    }

    // 状态结果相关
    public State OnSucceed(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.OnSucceedFunc = action);
    }

    public State OnFail(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.OnFailedFunc = action);
    }

    // 暂停/恢复相关
    public State OnPause(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.PauseFunc = action);
    }

    public State OnResume(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.ResumeFunc = action);
    }

    // 更新循环相关
    public State OnUpdate(Action<State, double> action)
    {
        return AddOrUpdateDelegate(c => c.UpdateFunc = action);
    }

    public State OnPhysicsUpdate(Action<State, double> action)
    {
        return AddOrUpdateDelegate(c => c.PhysicsUpdateFunc = action);
    }

    // 堆栈相关
    public State OnStack(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.OnStackFunc = action);
    }

    public State OnStackOverflow(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.OnStackOverflowFunc = action);
    }

    // 时间相关
    public State OnDurationOver(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.OnDurationOverFunc = action);
    }

    public State OnPeriodOver(Action<State> action)
    {
        return AddOrUpdateDelegate(c => c.OnPeriodOverFunc = action);
    }

    public State OnEnterCondition(Func<State, bool> condition)
    {
        return AddOrUpdateDelegate(c => c.EnterCondition = condition);
    }

    public State OnExitCondition(Func<State, bool> condition)
    {
        return AddOrUpdateDelegate(c => c.ExitCondition = condition);
    }

    // 辅助方法
    private State AddOrUpdateDelegate(Action<StandardDelegateComponent> setup)
    {
        if (!Components.TryGetValue(typeof(StandardDelegateComponent), out var component))
        {
            component = new StandardDelegateComponent();
            Components[typeof(StandardDelegateComponent)] = component;
        }
        setup((StandardDelegateComponent)component);
        return this;
}
}