using Godot;

public abstract class GameplayTagEventHandler
{
    public GameplayTag Tag { get; }
    
    protected GameplayTagEventHandler(GameplayTag tag)
    {
        Tag = tag;
    }
    
    // 当标签被添加时触发
    public virtual void OnTagAdded(GameplayTagContainer container, Node owner) { }
    
    // 当标签被移除时触发
    public virtual void OnTagRemoved(GameplayTagContainer container, Node owner) { }
    
    // 当标签存在时的每帧更新
    public virtual void OnTagUpdate(GameplayTagContainer container, Node owner, double delta) { }
}