using System;
using System.Collections.Generic;

namespace GPC.States;

public abstract class AbsState
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public Layer Layer { get; set; } = null;
    public int Priority { get; set; } = 0;
    public double IntervalTime { get; set; }
    public double IntervalElapsedTime { get; set; }
    public Type Type { get; set; }
    public JobRunningStatus Status { get; set; } = JobRunningStatus.NoRun;
    public bool UsePrepareFuncAsRunCondition { get; set; } = true;
    public Func<bool> IsPreparedFunc { get; set; }
    public Func<bool> IsSucceedFunc { get; set; }
    public Func<bool> IsFailedFunc { get; set; }

    public Dictionary<object, bool> SucceedEffects { get; set; }

    public Dictionary<object, bool> FailedEffects { get; set; }
    public Action<AbsState> StartFunc { get; set; }
    public Action<AbsState> SucceedFunc { get; set; }
    public Action<AbsState> FailedFunc { get; set; }
    public Action<AbsState> PauseFunc { get; set; }
    public Action<AbsState> ResumeFunc { get; set; }
    public Action<AbsState> RunningFunc { get; set; }
    public Action<AbsState> RunningPhysicsFunc { get; set; }
    public Action<AbsState> IntervalUpdateFunc { get; set; }

}