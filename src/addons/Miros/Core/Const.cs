using System;

namespace Miros.Core;

public enum CompareType
{
    Equals,
    Greater,
    Less
}

public enum RunningStatus
{
    None, //未运行
    Running, //运行中
    Succeed, //成功
    Failed, //失败
    Removed, //移除
    Paused //暂停
}

// 打断策略，目前只对 MultiLayerExecutor 有效
public enum InterruptPolicy
{
    None, //正常，被打断后结束任务，执行下一任务
    Fixed, //固定，不可被打断，只有执行完成后
    Fallback //回退，被当断后任务暂停，等待下一任务结束后恢复运行
}

public enum GrabType
{
    Source,
    Target
}

public enum StackType
{
    Source, //对Buff Source 限制层数
    Target //对Buff Target 限制层数
}

public enum RemovePolicy
{
    None, //不移除
    Condition, //自定义条件移除
    WhenFailed, //任务失败时移除
    WhenSucceed, //任务成功时移除
    WhenEnterFailed, //任务进入失败时移除
    WhenExited, //任务结束时移除（成功或失败）
    WhenSourceAgentNull, //任务的 Source Agent为空时移除
    WhenSourceStateRemoved, //任务的 Source Task被移除时移除
    WhenSourceStateExited, //任务的 Source Task结束时移除（成功或失败）
    WhenSourceStateFailed, //任务的 Source Task失败时移除
    WhenSourceStateSucceed //任务的 Source Task成功时移除
}

public enum GrantedAbilityActivationPolicy
{
    /// <summary>
    ///     不激活, 等待用户调用ASC激活
    /// </summary>
    None,

    /// <summary>
    ///     能力添加时激活（GE添加时激活）
    /// </summary>
    WhenAdded,

    /// <summary>
    ///     同步GE激活时激活
    /// </summary>
    SyncWithEffect
}

/// <summary>
///     授予能力的取消激活策略
/// </summary>
public enum GrantedAbilityDeactivationPolicy
{
    /// <summary>
    ///     无相关取消激活逻辑, 需要用户调用ASC取消激活
    /// </summary>
    None,

    /// <summary>
    ///     同步GE，GE失活时取消激活
    /// </summary>
    SyncWithEffect
}

/// <summary>
///     授予能力的移除策略
/// </summary>
public enum GrantedAbilityRemovePolicy
{
    /// <summary>
    ///     不移除
    /// </summary>
    None,

    /// <summary>
    ///     同步GE，GE移除时移除
    /// </summary>
    SyncWithEffect,

    /// <summary>
    ///     能力结束时自己移除
    /// </summary>
    WhenEnd,

    /// <summary>
    ///     能力取消时自己移除
    /// </summary>
    WhenCancel,

    /// <summary>
    ///     能力结束或取消时自己移除
    /// </summary>
    WhenCancelOrEnd
}

public enum StackingType
{
    None, //不会叠加，如果多次释放则每个Effect相当于单个Effect

    AggregateBySource, //目标(Target)上的每个源(Source)ASC都有一个单独的堆栈实例, 每个源(Source)可以应用堆栈中的X个GameplayEffect.

    AggregateByTarget //目标(Target)上只有一个堆栈实例而不管源(Source)如何, 每个源(Source)都可以在共享堆栈限制(Shared Stack Limit)内应用堆栈.
}

public enum DurationRefreshPolicy
{
    NeverRefresh, //不刷新Effect的持续时间

    RefreshOnSuccessfulApplication //每次apply成功后刷新Effect的持续时间, denyOverflowApplication如果为True则多余的Apply不会刷新Duration
}

public enum PeriodResetPolicy
{
    NeverRefresh, //不重置Effect的周期计时

    ResetOnSuccessfulApplication //每次apply成功后重置Effect的周期计时
}

public enum ExpirationPolicy
{
    ClearEntireStack, //持续时间结束时,清除所有层数

    RemoveSingleStackAndRefreshDuration, //持续时间结束时减少一层，然后重新经历一个Duration，一直持续到层数减为0

    RefreshDuration //持续时间结束时,再次刷新Duration，这相当于无限Duration，
    //TODO :可以通过调用GameplayEffectsContainer的OnStackCountChange(GameplayEffect ActiveEffect, int OldStackCount, int NewStackCount)来处理层数，
    //TODO :可以达到Duration结束时减少两层并刷新Duration这样复杂的效果。
}

public enum ModifierOperation
{
    Add = 0, //加
    Minus = 1, //减
    Multiply = 2, //乘
    Divide = 3, //除
    Override = 4, //覆盖
    Invalid = 5 //无效
}

[Flags]
public enum SupportedOperation
{
    None = 0,
    Add = 1 << 0, //加
    Multiply = 1 << 1, //乘
    Divide = 1 << 2, //除
    Override = 1 << 3, //覆盖
    All = Add | Multiply | Divide | Override //所有
}

public enum CalculateMode
{
    Stacking, //堆叠
    MinValueOnly, //最小值
    MaxValueOnly //最大值
}

public enum TagMatchType
{
    Explicit, // 精确匹配
    IncludeParentTags, // 包含父标签
    IncludeChildTags // 包含子标签
}

public enum AbilityActivateResult
{
    Success, //激活成功
    FailHasActivated, //已经激活
    FailTagRequirement, //标签不匹配
    FailCost, //消耗不足
    FailCooldown, //冷却不足
    FailOtherReason //其他原因
}

public enum DurationPolicy //持续策略
{
    Instant, //立即生效
    Infinite, //永久生效
    Duration, //规定时长
    Periodic //周期生效
}

public enum RunningCompletePolicy
{
    Remove, //移除
    Keep //保持
}