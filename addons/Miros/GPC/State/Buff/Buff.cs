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
///     叠加新buff时持续时间的更新策略
/// </summary>
public enum BuffStackDurationRefreshPolicy
{
    Reset, //重置
    Delay //延长
}

/// <summary>
///     叠加新buff时周期的更新策略
/// </summary>
public enum BuffStackPeriodResetPolicy
{
    Reset,//重置
    Delay 
}

public enum BuffStackingType
{
    None,
    Source, //对Buff Source 限制层数
    Target, //对Buff Target 限制层数
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
    public IGpcToken Source { get; set; }
    public IGpcToken Target { get; set; }
    
    // Core
    public BuffDurationPolicy DurationPolicy { get; set; } = BuffDurationPolicy.Instant;
    public List<Modifier> Modifiers { get; set; }

    // Period 
    public bool IsExecutePeriodicEffectOnStart { get; set; } = true;
    public BuffPeriodicInhibitionPolicy PeriodicInhibitionPolicy { get; set; } = BuffPeriodicInhibitionPolicy.Resume;

    // Chance
    public float Chance { get; set; } //0.0 ~ 1.0
    
    // Stacking
    public int StackMaxCount { get; set; } = 1;
    public int StackCurrentCount { get; set; } = 1;
    public BuffStackingType StackingType { get; set; } 
    public BuffStackDurationRefreshPolicy StackDurationRefreshPolicy { get; set; }
    public BuffStackPeriodResetPolicy StackPeriodResetPolicy { get; set; }
    public BuffStackExpirationPolicy StackExpirationPolicy { get; set; }
   
    // Stacking Overflow
    public List<AbsState> OnStackOverflowStates { get; set; }
    public bool IsClearStackOnOverflow { get; set; }
    
    // Expiration
    public List<AbsState> OnSucceedStates { get; set; }
    public List<AbsState> OnFailedStates { get; set; }
    
    // Immunity
}