// 标签事件参数
using System;
using System.Collections.Generic;

namespace Miros.Core;
public class TagEventArgs : EventArgs
{
    public Tag Tag { get; }
    public TagContainer Container { get; }
    
    public TagEventArgs(Tag tag, TagContainer container)
    {
        Tag = tag;
        Container = container;
    }
}

// 标签容器事件参数
public class TagContainerEventArgs : EventArgs
{
    public TagContainer Container { get; }
    public IEnumerable<Tag> Tags { get; }
    
    public TagContainerEventArgs(TagContainer container, IEnumerable<Tag> tags)
    {
        Container = container;
        Tags = tags;
    }
} 