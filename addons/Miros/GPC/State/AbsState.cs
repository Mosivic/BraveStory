using System;
using System.Collections.Generic;

namespace GPC.States;

public abstract class AbsState
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Layer Layer { get; set; } = null;
    public int Priority { get; set; } = 0;
    public double IntervalTime { get; set; }
    public double ElapsedTime { get; set; }
    public Type Type { get; set; }
    public Status Status { get; set; } = Status.Pause;
    public Func<bool> IsPreparedFunc { get; set; }
    public Func<bool> IsSucceedFunc { get; set; }
    public Func<bool> IsFailedFunc { get; set; }
    
    public Dictionary<object, bool> SucceedEffects { get; set; }

    public Dictionary<object, bool> FailedEffects { get; set; }
    public Action<AbsState> EnterFunc { get; set; }
    public Action<AbsState> ExitFunc { get; set; }
    public Action<AbsState> PauseFunc { get; set; }
    public Action<AbsState> ResumeFunc { get; set; }
    public Action<AbsState> RunningFunc { get; set; }
    public Action<AbsState> RunningPhysicsFunc { get; set; }
    public Action<AbsState> IntervalUpdateFunc { get; set; }
    public Action<AbsState> EnterAttachFunc { get; set; }
    public Action<AbsState> ExitAttachFunc { get; set; }
    public Action<AbsState> PauseAttachFunc { get; set; }
    public Action<AbsState> ResumeAttachFunc { get; set; }
    public Action<AbsState> RunningAttachFunc { get; set; }
    public Action<AbsState> RunningPhysicsAttachFunc { get; set; }
    public Action<AbsState> AttachIntervalUpdateFunc { get; set; }
}