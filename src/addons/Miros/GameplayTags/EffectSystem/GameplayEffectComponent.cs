using System.Collections.Generic;
using Godot;

public partial class GameplayEffectComponent : Node
{
    private GameplayTagContainer _ownerTags = new([]);
    private Dictionary<GameplayEffect, double> _activeEffects = new();
    
    public override void _Ready()
    {
        // 订阅标签变化事件
        _ownerTags.TagAdded += OnTagAdded;
        _ownerTags.TagRemoved += OnTagRemoved;
    }
    
    public override void _Process(double delta)
    {
        // 更新效果持续时间
        var expiredEffects = new List<GameplayEffect>();
        
        foreach (var kvp in _activeEffects)
        {
            var effect = kvp.Key;
            var remainingTime = kvp.Value - delta;
            
            if (remainingTime <= 0)
            {
                expiredEffects.Add(effect);
            }
            else
            {
                _activeEffects[effect] = remainingTime;
            }
        }
        
        // 移除过期效果
        foreach (var effect in expiredEffects)
        {
            RemoveEffect(effect);
        }
    }
    
    public bool TryApplyEffect(GameplayEffect effect)
    {
        // 检查是否满足标签要求
        if (!CanApplyEffect(effect)) return false;
        
        // 应用效果
        _activeEffects[effect] = effect.Duration;
        
        // 添加效果赋予的标签
        foreach (var tag in effect.GrantedTags.GetAllTags())
        {
            _ownerTags.AddTag(tag);
        }
        
        return true;
    }
    
    private bool CanApplyEffect(GameplayEffect effect)
    {
        // 检查必需标签
        if (!_ownerTags.HasAll(effect.RequiredTags))
            return false;
            
        // 检查忽略标签
        if (_ownerTags.HasAny(effect.IgnoredTags))
            return false;
            
        return true;
    }
    
    private void RemoveEffect(GameplayEffect effect)
    {
        if (_activeEffects.Remove(effect))
        {
            // 移除效果赋予的标签
            foreach (var tag in effect.GrantedTags.GetAllTags())
            {
                _ownerTags.RemoveTag(tag);
            }
        }
    }
    
    private void OnTagAdded(object sender, GameplayTagEventArgs e)
    {
        // 可以在这里处理标签添加时的特殊逻辑
        GD.Print($"Effect system: Tag added - {e.Tag}");
    }
    
    private void OnTagRemoved(object sender, GameplayTagEventArgs e)
    {
        // 可以在这里处理标签移除时的特殊逻辑
        GD.Print($"Effect system: Tag removed - {e.Tag}");
    }
}