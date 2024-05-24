using System;
using System.Collections.Generic;
using Godot;

namespace GPC.Job.Config;

public class State : IState, IHubProvider
{
    public int ChildIndex { get; set; } = -1;
    public int Cost { get; set; }
    public int Priority { get; set; }
    public Dictionary<string, object> Args { get; set; }

    public Dictionary<string, object> Extend { get; set; }

    public Dictionary<object, object> Desired { get; set; }

    public List<State> Subjobs { get; set; }

    public Dictionary<object, bool> Preconditions { get; set; }

    public Dictionary<object, bool> SuccessedConditions { get; set; }

    public Dictionary<object, bool> FailedConditions { get; set; }

    public Dictionary<object, bool> SuccessedEffects { get; set; }

    public Dictionary<object, bool> FailedEffects { get; set; }

    public Action<State> EnterFunc { get; set; }

    public Action<State> ExitFunc { get; set; }

    public Action<State> PauseFunc { get; set; }

    public Action<State> ResumeFunc { get; set; }

    public Action<State> RunningFunc { get; set; }

    public Action<State> RunningPhysicsFunc { get; set; }

    public Predicate<State> IsPreparedFunc { get; set; }

    public Predicate<State> IsSucceedFunc { get; set; }

    public Predicate<State> IsFailedFunc { get; set; }

    public Action<State> EnterAttachFunc { get; set; }

    public Action<State> ExitAttachFunc { get; set; }

    public Action<State> PauseAttachFunc { get; set; }

    public Action<State> ResumeAttachFunc { get; set; }

    public Action<State> RunningAttachFunc { get; set; }

    public Action<State> RunningPhysicsAttachFunc { get; set; }

    public Action<State> RunningInterval { get; set; }
    public float Interval { get; set; }
    public IHub Hub => GHub.GetIns();
    public string Id { get; set; }
    public string Layer { get; set; } = "Defult";
    public Type Type { get; set; }
    public Status Status { get; set; } = Status.Running;
}