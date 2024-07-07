using System;
using System.Collections.Generic;
using GPC.Scheduler;

namespace GPC.States.Buff;

/// <summary>
/// 持续策略
/// </summary>
public enum BuffDurationPolicy //持续策略
{
    Instand, //立即生效
    Infinite, //永久生效
    Interval //规定时长
}

/// <summary>
/// Buff中断并恢复的处理策略
/// </summary>
public enum BuffPeriodicInhibitionPolicy 
{
    Resume, //恢复衔接
    Reset, //重置
    ExecuteAndReset //执行一次后再重置
}

/// <summary>
/// 叠加新buff时持续时间的更新策略
/// </summary>
internal enum BuffStackDurationRefreshPolicy 
{
    Reset, //重置
    Delay //延长
}

/// <summary>
/// 叠加新buff时周期的更新策略
/// </summary>
internal enum BuffStackPeriodResetPolicy 
{
    Reset,
    Delay
}
/// <summary>
/// 当一层buff到期后的处理策略
/// </summary>
internal enum BuffStackExpirationPolicy 
{
    ClearAllStack,
    RemoveOneStackAndRefreshDuration,
    RefreshDuration
}

public class Buff : AbsState
{
    public int StackMaxCount { get; set; }= 1;
    public float Period { get; set; }
    
    public IGpcToken Source { get; set; }
    public IGpcToken Target { get; set; }
    public List<Modifier> Modifiers { get; set; }
    
    public BuffPeriodicInhibitionPolicy PeriodicInhibitionPolicy { get; set; } = BuffPeriodicInhibitionPolicy.Resume;
    public BuffDurationPolicy DurationPolicy { get; set; } = BuffDurationPolicy.Instand;
    //层数溢出后调用的Buff
    public List<Buff> OverflowBuffs { get; set; } 
    
}