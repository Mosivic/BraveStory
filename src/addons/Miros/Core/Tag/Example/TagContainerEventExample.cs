using System.Collections.Generic;
using Godot;
using Miros.Core;

public class TagEventContainerExample
{
    private TagContainer _tags;
    private TagManager _tagManager;
    
    public void Initialize()
    {
        _tagManager = TagManager.Instance;
        _tags = new TagContainer([]);
        
        // 订阅事件
        _tags.TagAdded += OnTagAdded;
        _tags.TagRemoved += OnTagRemoved;
        _tags.TagsChanged += OnTagsChanged;
    }
    
    private void OnTagAdded(object sender, GameplayTagEventArgs e)
    {
        GD.Print($"Tag added: {e.Tag}");
        
        // 示例：当添加眩晕标签时应用效果
        if (e.Tag == _tagManager.RequestGameplayTag("Status.Stun"))
        {
            ApplyStunEffect();
        }
    }
    
    private void OnTagRemoved(object sender, GameplayTagEventArgs e)
    {
        GD.Print($"Tag removed: {e.Tag}");
        
        // 示例：当移除眩晕标签时移除效果
        if (e.Tag == _tagManager.RequestGameplayTag("Status.Stun"))
        {
            RemoveStunEffect();
        }
    }
    
    private void OnTagsChanged(object sender, GameplayTagContainerEventArgs e)
    {
        GD.Print($"Tags changed: {string.Join(", ", e.Tags)}");
        
        // 示例：检查状态变化
        CheckStatusEffects(e.Tags);
    }
    
    private void ApplyStunEffect()
    {
        // 实现眩晕效果
    }
    
    private void RemoveStunEffect()
    {
        // 移除眩晕效果
    }
    
    private void CheckStatusEffects(IEnumerable<Tag> changedTags)
    {
        // 检查并处理状态效果变化
    }
}