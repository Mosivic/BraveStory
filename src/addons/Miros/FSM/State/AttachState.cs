using System;

namespace FSM.States;

public class AttachState : AbsState
{
    public Action<AbsState> StartAttachFunc { get; set; }
    public Action<AbsState> SucceedAttachFunc { get; set; }
    public Action<AbsState> FailedAttachFunc { get; set; }
    public Action<AbsState> PauseAttachFunc { get; set; }
    public Action<AbsState> ResumeAttachFunc { get; set; }
    public Action<AbsState> RunningAttachFunc { get; set; }
    public Action<AbsState> RunningPhysicsAttachFunc { get; set; }
    public Action<AbsState> AttachIntervalUpdateFunc { get; set; }
}