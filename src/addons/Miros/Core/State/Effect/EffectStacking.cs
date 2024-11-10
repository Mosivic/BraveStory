using System;

namespace Miros.Core;

// GE堆栈数据结构
public struct EffectStacking
{
    public string StackingCodeName { get; set; } // 实际允许不会使用，而是使用stackingCodeName的hash值, 即stackingHashCode
    public int StackingHashCode { get => StackingCodeName?.GetHashCode() ?? 0; set => StackingCodeName = value.ToString(); }
    public StackingType StackingType { get; set; }
    public int LimitCount { get; set; }
    public DurationRefreshPolicy DurationRefreshPolicy { get; set; }
    public PeriodResetPolicy PeriodResetPolicy { get; set; }
    public ExpirationPolicy ExpirationPolicy { get; set; }

    // Overflow 溢出逻辑处理
    public bool DenyOverflowApplication { get; set; } //对应于StackDurationRefreshPolicy，如果为True则多余的Apply不会刷新Duration
    public bool ClearStackOnOverflow { get; set; } //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
    public Effect[] OverflowEffects { get; set; } // 超过StackLimitCount数量的Effect被Apply时将会调用该OverflowEffects


    public static EffectStacking None
    {
        get
        {
            var stack = new EffectStacking();
            stack.StackingType = StackingType.None;
            return stack;
        }
    }
}


