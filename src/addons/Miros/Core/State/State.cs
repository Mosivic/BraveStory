using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using BraveStory;

namespace Miros.Core;


public struct Transition
{
    public Tag ToStateSign;
    public Func<bool> Condition;
    public StateTransitionMode Mode;
}


public class State(Tag sign)
{
    public Tag Sign { get; init; } = sign;

    public Type TaskType { get; init; } = typeof(TaskBase);
    public int Priority { get; init; } = 0;

    public Persona Owner { get; protected set; }
    public Persona Source { get; protected set; }

    public RunningStatus Status { get; set; } = RunningStatus.NoRun;
    public bool IsActive => Status == RunningStatus.Running;

    public double RunningTime { get; set; } = 0;

    public HashSet<Transition> Transitions { get; set; } = [];


    public Dictionary<Type, StateComponent<TaskBase>> Components { get; set; } = [];

    // 添加便捷的组件添加方法
    public State AddComponent<T>(Action<T> setup = null) where T : StateComponent<TaskBase>, new()
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
        GetStandardDelegateComponent().EnterFunc += action;
        return this;
    }

    public State OnEnterIf(Func<State, bool> condition)
    {
        GetStandardDelegateComponent().EnterCondition += condition;
        return this;
    }

    // 退出相关
    public State OnExit(Action<State> action)
    {
        GetStandardDelegateComponent().ExitFunc += action;
        return this;
    }

    public State OnExitIf(Func<State, bool> condition)
    {
        GetStandardDelegateComponent().ExitCondition += condition;
        return this;
    }

    // 状态结果相关
    public State OnSucceed(Action<State> action)
    {
        GetStandardDelegateComponent().OnSucceedFunc += action;
        return this;
    }

    public State OnFail(Action<State> action)
    {
        GetStandardDelegateComponent().OnFailedFunc += action;
        return this;
    }

    // 暂停/恢复相关
    public State OnPause(Action<State> action)
    {
        GetStandardDelegateComponent().PauseFunc += action;
        return this;
    }

    public State OnResume(Action<State> action)
    {
        GetStandardDelegateComponent().ResumeFunc += action;
        return this;
    }

    // 更新循环相关
    public State OnUpdate(Action<State, double> action)
    {
        GetStandardDelegateComponent().UpdateFunc += action;
        return this;
    }

    public State OnPhysicsUpdate(Action<State, double> action)
    {
        GetStandardDelegateComponent().PhysicsUpdateFunc += action;
        return this;
    }

    // 堆栈相关
    public State OnStack(Action<State> action)
    {
        GetStandardDelegateComponent().OnStackFunc += action;
        return this;
    }

    public State OnStackOverflow(Action<State> action)
    {
        GetStandardDelegateComponent().OnStackOverflowFunc += action;
        return this;
    }

    // 时间相关
    public State OnDurationOver(Action<State> action)
    {
        GetStandardDelegateComponent().OnDurationOverFunc += action;
        return this;
    }

    public State OnPeriodOver(Action<State> action)
    {
        GetStandardDelegateComponent().OnPeriodOverFunc += action;
        return this;
    }

    public State OnEnterCondition(Func<State, bool> condition)
    {
        GetStandardDelegateComponent().EnterCondition += condition;
        return this;
    }

    public State OnExitCondition(Func<State, bool> condition)
    {
        GetStandardDelegateComponent().ExitCondition += condition;
        return this;
    }

    public State To(Tag sign, Func<bool> condition = null, StateTransitionMode mode = StateTransitionMode.Normal)
    {
        Transitions.Add(new Transition { ToStateSign = sign, Condition = condition, Mode = mode });
        return this;
    }

    public State Any(Func<bool> condition = null, StateTransitionMode mode = StateTransitionMode.Normal)
    {
        Transitions.Add(new Transition { ToStateSign = Tags.None, Condition = condition, Mode = mode });
        return this;
    }

    // 辅助方法
    private StandardDelegateComponent GetStandardDelegateComponent()
    {
        if (!Components.TryGetValue(typeof(StandardDelegateComponent), out var component))
        {
            component = new StandardDelegateComponent();
            Components[typeof(StandardDelegateComponent)] = component;
        }
        return (StandardDelegateComponent)component;
    }
}