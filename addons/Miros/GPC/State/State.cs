﻿using System;
using System.Collections.Generic;
using GPC.Scheduler;

namespace GPC.States;

public class State : AbsState
{
    public int ChildIndex { get; set; } = -1;
    public int Cost { get; set; }
    public Dictionary<string, object> Args { get; set; }

    public Dictionary<string, object> Extend { get; set; }

    public Dictionary<object, object> Desired { get; set; }
    public List<State> Subjobs { get; set; }
    public List<ConditionBase> PreCondition { get; set; }
    public List<ConditionBase> SuccessedCondition { get; set; }
    public List<ConditionBase> FailedCondition { get; set; }

    public Dictionary<object, bool> SuccessedEffects { get; set; }

    public Dictionary<object, bool> FailedEffects { get; set; }
    public Action<AbsState> EnterFunc { get; set; }

    public Action<AbsState> ExitFunc { get; set; }

    public Action<AbsState> PauseFunc { get; set; }

    public Action<AbsState> ResumeFunc { get; set; }

    public Action<AbsState> RunningFunc { get; set; }

    public Action<AbsState> RunningPhysicsFunc { get; set; }

    public Predicate<AbsState> IsPreparedFunc { get; set; }

    public Predicate<AbsState> IsSucceedFunc { get; set; }

    public Predicate<AbsState> IsFailedFunc { get; set; }

    public Action<AbsState> EnterAttachFunc { get; set; }

    public Action<AbsState> ExitAttachFunc { get; set; }
    public Action<AbsState> PauseAttachFunc { get; set; }
    public Action<AbsState> ResumeAttachFunc { get; set; }
    public Action<AbsState> RunningAttachFunc { get; set; }
    public Action<AbsState> RunningPhysicsAttachFunc { get; set; }
    public Action<AbsState> RunningInterval { get; set; }
    public float Interval { get; set; }
}