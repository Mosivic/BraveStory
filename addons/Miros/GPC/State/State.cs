using System;
using System.Collections.Generic;

namespace GPC.States;

public class State : AbsState
{
    public int ChildIndex { get; set; } = -1;
    public int Cost { get; set; }
    public float Interval { get; set; }
    
    public Dictionary<object, object> Desired { get; set; }
    public List<AbsState> SubJobs { get; set; }
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
    public Action<AbsState> EnterAttachFunc { get; set; }
    public Action<AbsState> ExitAttachFunc { get; set; }
    public Action<AbsState> PauseAttachFunc { get; set; }
    public Action<AbsState> ResumeAttachFunc { get; set; }
    public Action<AbsState> RunningAttachFunc { get; set; }
    public Action<AbsState> RunningPhysicsAttachFunc { get; set; }
    public Action<AbsState> RunningInterval { get; set; }

}