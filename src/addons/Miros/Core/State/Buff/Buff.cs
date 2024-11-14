using System;
using System.Collections.Generic;

namespace Miros.Core;

/// <summary>
///     Buff中断并恢复的处理策略
/// </summary>
public enum PeriodicInhibitionPolicy
{
    Resume, //恢复衔接
    Reset, //重置
    ExecuteAndReset //执行一次后再重置
}

/// <summary>
///     当一层buff到期后的处理策略
/// </summary>
public enum StackExpirationPolicy
{
    ClearAllStack,
    RemoveOneStackAndRefreshDuration,
    RefreshDuration
}

public class Buff(string name, Tag sign) : State(name, sign)
{
    // Core
    public DurationPolicy DurationPolicy { get; set; } = DurationPolicy.Instant;
    public List<Modifier> Modifiers { get; set; } = new();

    // Period 
    public bool IsExecutePeriodicEffectOnStart { get; set; } = true;
    public PeriodicInhibitionPolicy PeriodicInhibitionPolicy { get; set; } = PeriodicInhibitionPolicy.Resume;

    // Chance
    public bool HasChance { get; init; } = false;
    public float Chance { get; init; } = 1.0f; //0.0 ~ 1.0


    // Duration
    public double Duration { get; init; } = 0;

    public double DurationElapsed { get; set; }

    // Period
    public double Period { get; set; } = 0;
    public double PeriodElapsed { get; set; }

    // Stack
    public int StackMaxCount { get; init; } = 1;
    public int StackCurrentCount { get; set; } = 1;
    public Dictionary<object, int> StackSourceCountDict { get; set; }
    public bool IsStack { get; init; } = false;
    public StateStackType StackType { get; init; } = StateStackType.Target;

    // Stacking
    public bool StackIsRefreshDuration { get; init; } = false;
    public bool StackIsResetPeriod { get; init; } = false;
    public StackExpirationPolicy StackExpirationPolicy { get; set; }

    // Stacking Overflow
    public List<State> OnStackOverflowStates { get; set; }
    public bool IsClearStackOnOverflow { get; set; }

    // Expiration
    public List<State> OnSucceedStates { get; set; }
    public List<State> OnFailedStates { get; set; }

    // Immunity

    //Function
    public Action<Modifier> OnApplyModifierFunc { get; init; }
    public Action<Modifier> OnCancelModifierFunc { get; init; }
}