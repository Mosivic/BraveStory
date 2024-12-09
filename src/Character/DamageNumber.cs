using Godot;
using System;

namespace BraveStory;

public partial class DamageNumber : Node2D
{
    private Label _label;
    private Tween _tween;
    
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        
        // 创建上浮和淡出动画
        _tween = CreateTween();
        _tween.SetParallel(true);
        
        // 向上移动
        _tween.TweenProperty(this, "position", 
            Position + new Vector2(0, -100), 0.8f);
            
        // 淡出效果
        _tween.TweenProperty(_label, "modulate:a", 0.0f, 0.8f).SetEase(Tween.EaseType.Out);
        
        // 动画完成后删除节点
        _tween.TweenCallback(Callable.From(() => QueueFree()));
    }
    
    public void SetDamage(int damage)
    {
        _label.Text = damage.ToString();
    }
}

