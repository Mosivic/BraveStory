using System;
using System.Collections.Generic;

namespace Miros.Core;


public class State(Tag tag,Agent source)
{
    public Tag Tag { get; init; } = tag;
    public virtual Type Type => typeof(TaskBase);
    public int Priority { get; init; } = 0;
    public Agent Owner { get;  set; }
    public Agent Source { get; init; } = source;

    public RunningStatus Status { get; set; } = RunningStatus.NoRun;
    public bool IsActive => Status == RunningStatus.Running;

    public double RunningTime { get; set; } = 0;


    public Dictionary<Type, StateComponent<TaskBase>> Components { get; set; } = [];


    public State AddComponent<T>(Action<T> setup = null) where T : StateComponent<TaskBase>, new()
    {
        var component = new T();
        setup?.Invoke(component);
        Components[typeof(T)] = component;
        return this;
    }

    public State OnEntered(Action<State> action)
    {
        GetStandardDelegateComponent().EnterFunc += action;
        return this;
    }

    public State EnterCondition(Func<State, bool> action)
    {
        GetStandardDelegateComponent().EnterCondition += action;
        return this;
    }

    public State OnExited(Action<State> action)
    {
        GetStandardDelegateComponent().ExitFunc += action;
        return this;
    }

    public State ExitCondition(Func<State, bool> condition)
    {
        GetStandardDelegateComponent().ExitCondition += condition;
        return this;
    }

    public State OnSucceed(Action<State> action)
    {
        GetStandardDelegateComponent().OnSucceedFunc += action;
        return this;
    }

    public State OnFailed(Action<State> action)
    {
        GetStandardDelegateComponent().OnFailedFunc += action;
        return this;
    }

    public State OnPaused(Action<State> action)
    {
        GetStandardDelegateComponent().PauseFunc += action;
        return this;
    }

    public State OnResumed(Action<State> action)
    {
        GetStandardDelegateComponent().ResumeFunc += action;
        return this;
    }

    public State OnUpdated(Action<State, double> action)
    {
        GetStandardDelegateComponent().UpdateFunc += action;
        return this;
    }

    public State OnPhysicsUpdated(Action<State, double> action)
    {
        GetStandardDelegateComponent().PhysicsUpdateFunc += action;
        return this;
    }

    // 堆栈相关
    public State OnStacked(Action<State> action)
    {
        GetStandardDelegateComponent().OnStackFunc += action;
        return this;
    }

    public State OnStackOverflowed(Action<State> action)
    {
        GetStandardDelegateComponent().OnStackOverflowFunc += action;
        return this;
    }

    // 时间相关
    public State OnDurationOvered(Action<State> action)
    {
        GetStandardDelegateComponent().OnDurationOverFunc += action;
        return this;
    }

    public State OnPeriodOvered(Action<State> action)
    {
        GetStandardDelegateComponent().OnPeriodOverFunc += action;
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