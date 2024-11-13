using System;

namespace Miros.Core;

// GE堆栈数据结构
public class StackingComponent : StateComponent<EffectJob>
{
    public required Tag StackingGroupTag { get; set; } // 堆叠组标签, 用于区分不同的堆栈组,同一组的 Effect 可以共同触发堆叠
    public StackingType StackingType { get; set; } // 堆叠类型
    public int LimitCount { get; set; } // 堆叠上限
    public DurationRefreshPolicy DurationRefreshPolicy { get; set; } // 堆叠时长刷新策略
    public PeriodResetPolicy PeriodResetPolicy { get; set; } // 堆叠周期重置策略
    public ExpirationPolicy ExpirationPolicy { get; set; } // 堆叠过期策略

    // Overflow 溢出逻辑处理
    public bool DenyOverflowApplication { get; set; } //对应于StackDurationRefreshPolicy，如果为True则多余的Apply不会刷新Duration
    public bool ClearStackOnOverflow { get; set; } //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
    public Effect[] OverflowEffects { get; set; } // 超过StackLimitCount数量的Effect被Apply时将会调用该OverflowEffects
    
}


