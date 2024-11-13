using Godot;
using Miros.Core;
public partial class TagEventExample : Node
{
    private TagContainer _tags;
    
    public override void _Ready()
    {
        _tags = new TagContainer([]);
        
        // 注册事件处理器
        var eventManager = TagEventManager.Instance;
        eventManager.RegisterEventHandler(new StunnedTagHandler());
        eventManager.RegisterEventHandler(new BurningTagHandler());
        
        // 注册容器及其所有者
        eventManager.RegisterContainer(_tags, this);
    }
    
    public override void _Process(double delta)
    {
        // 更新标签事件
        TagEventManager.Instance.Update(delta);
    }
    
    public override void _ExitTree()
    {
        // 清理
        TagEventManager.Instance.UnregisterContainer(_tags);
    }
    
    // 测试方法
    public void TestStun()
    {
        var stunnedTag = TagManager.Instance.RequestTag("Status.Stunned");
        _tags.AddTag(stunnedTag);
        
        // 3秒后解除眩晕
        GetTree().CreateTimer(3.0).Connect("timeout", new Callable(this, nameof(RemoveStun)));
    }
    
    private void RemoveStun()
    {
        var stunnedTag = TagManager.Instance.RequestTag("Status.Stunned");
        _tags.RemoveTag(stunnedTag);
    }
}