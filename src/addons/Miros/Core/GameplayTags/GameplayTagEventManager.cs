using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class GameplayTagEventManager
{
    private static GameplayTagEventManager _instance;
    public static GameplayTagEventManager Instance => _instance ??= new GameplayTagEventManager();
    
    private readonly Dictionary<GameplayTag, GameplayTagEventHandler> _eventHandlers = new();
    private readonly Dictionary<GameplayTagContainer, Node> _containerOwners = new();
    
    // 注册事件处理器
    public void RegisterEventHandler(GameplayTagEventHandler handler)
    {
        _eventHandlers[handler.Tag] = handler;
    }
    
    // 注册容器及其所有者
    public void RegisterContainer(GameplayTagContainer container, Node owner)
    {
        _containerOwners[container] = owner;
        container.TagAdded += OnTagAdded;
        container.TagRemoved += OnTagRemoved;
    }
    
    // 取消注册容器
    public void UnregisterContainer(GameplayTagContainer container)
    {
        _containerOwners.Remove(container);
        container.TagAdded -= OnTagAdded;
        container.TagRemoved -= OnTagRemoved;
    }
    
    // 处理标签添加事件
    private void OnTagAdded(object sender, GameplayTagEventArgs e)
    {
        if (sender is GameplayTagContainer container && 
            _eventHandlers.TryGetValue(e.Tag, out var handler) &&
            _containerOwners.TryGetValue(container, out var owner))
        {
            handler.OnTagAdded(container, owner);
        }
    }
    
    // 处理标签移除事件
    private void OnTagRemoved(object sender, GameplayTagEventArgs e)
    {
        if (sender is GameplayTagContainer container && 
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