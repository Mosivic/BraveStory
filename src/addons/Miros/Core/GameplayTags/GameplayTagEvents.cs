// 标签事件参数
using System;
using System.Collections.Generic;

namespace Miros.Core;
public class GameplayTagEventArgs : EventArgs
{
    public GameplayTag Tag { get; }
    public GameplayTagContainer Container { get; }
    
    public GameplayTagEventArgs(GameplayTag tag, GameplayTagContainer container)
    {
        Tag = tag;
        Container = container;
    }
}

// 标签容器事件参数
public class GameplayTagContainerEventArgs : EventArgs
{
    public GameplayTagContainer Container { get; }
    public IEnumerable<GameplayTag> Tags { get; }
    
    public GameplayTagContainerEventArgs(GameplayTagContainer container, IEnumerable<GameplayTag> tags)
    {
        Container = container;
        Tags = tags;
    }
} 