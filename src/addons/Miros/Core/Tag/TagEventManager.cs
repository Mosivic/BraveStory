using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class TagEventManager
{
    private static TagEventManager _instance;
    public static TagEventManager Instance => _instance ??= new TagEventManager();
    
    private readonly Dictionary<Tag, TagEventHandler> _eventHandlers = new();
    private readonly Dictionary<TagContainer, Node> _containerOwners = new();
    
    // 注册事件处理器
    public void RegisterEventHandler(TagEventHandler handler)
    {
        _eventHandlers[handler.Tag] = handler;
    }
    
    // 注册容器及其所有者
    public void RegisterContainer(TagContainer container, Node owner)
    {
        _containerOwners[container] = owner;
        container.TagAdded += OnTagAdded;
        container.TagRemoved += OnTagRemoved;
    }
    
    // 取消注册容器
    public void UnregisterContainer(TagContainer container)
    {
        _containerOwners.Remove(container);
        container.TagAdded -= OnTagAdded;
        container.TagRemoved -= OnTagRemoved;
    }
    
    // 处理标签添加事件
    private void OnTagAdded(object sender, TagEventArgs e)
    {
        if (sender is TagContainer container && 
            _eventHandlers.TryGetValue(e.Tag, out var handler) &&
            _containerOwners.TryGetValue(container, out var owner))
        {
            handler.OnTagAdded(container, owner);
        }
    }
    
    // 处理标签移除事件
    private void OnTagRemoved(object sender, TagEventArgs e)
    {
        if (sender is TagContainer container && 
            _eventHandlers.TryGetValue(e.Tag, out var handler) &&
            _containerOwners.TryGetValue(container, out var owner))
        {
            handler.OnTagRemoved(container, owner);
        }
    }
    
    // 更新所有活动标签
    public void Update(double delta)
    {
        foreach (var containerOwner in _containerOwners)
        {
            var container = containerOwner.Key;
            var owner = containerOwner.Value;
            
            foreach (var tag in container.GetAllTags())
            {
                if (_eventHandlers.TryGetValue(tag, out var handler))
                {
                    handler.OnTagUpdate(container, owner, delta);
                }
            }
        }
    }
}