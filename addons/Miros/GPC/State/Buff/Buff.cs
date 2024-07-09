using System;
using System.Collections.Generic;

namespace GPC.States.Buff;

/// <summary>
///     持续策略
/// </summary>
public enum BuffDurationPolicy //持续策略
{
    Instant, //立即生效
    Infinite, //永久生效
    Duration //规定时长
}

/// <summary>
///     Buff中断并恢复的处理策略
/// </summary>
public enum BuffPeriodicInhibitionPolicy
{
    Resume, //恢复衔接
    Reset, //重置
    ExecuteAndReset //执行一次后再重置
}


/// <summary>
///     当一层buff到期后的处理策略
/// </summary>
public enum BuffStackExpirationPolicy
{
    ClearAllStack, 
    RemoveOneStackAndRefreshDuration,
    RefreshDuration
}


public class Buff : AbsState
{
    public  IGpcToken Source { get; set; }
    
    // Core
    public BuffDurationPolicy DurationPolicy { get; set; } = BuffDurationPolicy.Instant;
    public List<Modifier> Modifiers { get; set; }

    // Period 
    public bool IsExecutePeriodicEffectOnStart { get; set; } = true;
    public BuffPeriodicInhibitionPolicy PeriodicInhibitionPolicy { get; set; } = BuffPeriodicInhibitionPolicy.Resume;

    // Chance
    public float Chance { get; set; } //0.0 ~ 1.0
    
    // Stacking
    public bool StackIsRefreshDuration { get; set; } = false;
    public bool StackIsResetPeriod { get; set; } = false;
    public BuffStackExpirationPolicy StackExpirationPolicy { get; set; }
   
    // Stacking Overflow
    public List<AbsState> OnStackOverflowStates { get; set; }
    public bool IsClearStackOnOverflow { get; set; }
    
    // Expiration
    public List<AbsState> OnSucceedStates { get; set; }
    public List<AbsState> OnFailedStates { get; set; }
    
    // Immunity

}