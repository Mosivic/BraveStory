using Godot;
namespace Miros.Core;

public abstract class TagEventHandler
{
    public Tag Tag { get; }
    
    protected TagEventHandler(Tag tag)
    {
        Tag = tag;
    }
    
    // 当标签被添加时触发
    public virtual void OnTagAdded(TagContainer container, Node owner) { }
    
    // 当标签被移除时触发
    public virtual void OnTagRemoved(TagContainer container, Node owner) { }
    
    // 当标签存在时的每帧更新
    public virtual void OnTagUpdate(TagContainer container, Node owner, double delta) { }
}