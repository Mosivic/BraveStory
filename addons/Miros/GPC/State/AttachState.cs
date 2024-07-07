using System;

namespace GPC.States;

public class AttachState:AbsState
{
    public Action<AbsState> EnterAttachFunc { get; set; }
    public Action<AbsState> ExitAttachFunc { get; set; }
    public Action<AbsState> BreakAttachFunc { get; set; }
    public Action<AbsState> PauseAttachFunc { get; set; }
    public Action<AbsState> ResumeAttachFunc { get; set; }
    public Action<AbsState> RunningAttachFunc { get; set; }
    public Action<AbsState> RunningPhysicsAttachFunc { get; set; }
    public Action<AbsState> AttachIntervalUpdateFunc { get; set; }
}