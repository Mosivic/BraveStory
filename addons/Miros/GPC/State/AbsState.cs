using System;
using System.Collections.Generic;
using Godot;
using GPC.Scheduler;

namespace GPC.State;

public interface IStateGeneric<H>
{
    public H Host { get; set; }
}

public class AbsState : StateExtendProperty, IState
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Layer { get; set; }
    public int Priority { get; set; }
    public Type Type { get; set; }
    public Status Status { get; set; }
    public IScheduler Scheduler { get; set; }

}

public class StateExtendProperty
{
    public int ChildIndex { get; set; } = -1;
    public int Cost { get; set; }
    public Dictionary<string, object> Args { get; set; }

    public Dictionary<string, object> Extend { get; set; }

    public Dictionary<object, object> Desired { get; set; }
    public List<AbsState> Subjobs { get; set; }
    public List<IEvaluator> PreCondition { get; set; }
    public Condition SuccessedCondition { get; set; }
    public Condition FailedCondition { get; set; }

    public Dictionary<object, bool> SuccessedEffects { get; set; }

    public Dictionary<object, bool> FailedEffects { get; set; }

    public Action<IState> EnterFunc { get; set; }

    public Action<IState> ExitFunc { get; set; }

    public Action<IState> PauseFunc { get; set; }

    public Action<IState> ResumeFunc { get; set; }

    public Action<IState> RunningFunc { get; set; }

    public Action<IState> RunningPhysicsFunc { get; set; }

    public Predicate<IState> IsPreparedFunc { get; set; }

    public Predicate<IState> IsSucceedFunc { get; set; }

    public Predicate<IState> IsFailedFunc { get; set; }

    public Action<IState> EnterAttachFunc { get; set; }

    public Action<IState> ExitAttachFunc { get; set; }

    public Action<IState> PauseAttachFunc { get; set; }

    public Action<IState> ResumeAttachFunc { get; set; }

    public Action<IState> RunningAttachFunc { get; set; }

    public Action<IState> RunningPhysicsAttachFunc { get; set; }

    public Action<IState> RunningInterval { get; set; }
    public float Interval { get; set; }
}


