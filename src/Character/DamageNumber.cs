using Godot;
using System;

namespace BraveStory;

public partial class DamageNumber : Node2D
{
    private Label _label;
    private Tween _tween;

    [Export]
    private float lifetime = 0.5f; // 设置伤害数字的显示时间
    [Export]
    private float moveDistance = 20; // 设置伤害数字的移动距离
    [Export]
    private Color color = Colors.Red; // 设置伤害数字的颜色



    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _label.AddThemeColorOverride("font_color", color);


        // 创建 Tween 实例
        _tween = CreateTween();
        _tween.SetParallel(true);
        
        // 向上移动
        _tween.TweenProperty(this, "position", 
            Position + new Vector2(0, -moveDistance), lifetime);
        
        // 淡出效果
        _tween.TweenProperty(_label, "modulate:a", 0.0f, lifetime).SetEase(Tween.EaseType.Out);
        
        // 缩放效果
        _tween.TweenProperty(this, "scale", new Vector2(1.5f, 1.5f), 0.2f).SetTrans(Tween.TransitionType.Bounce);
        _tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.2f).SetTrans(Tween.TransitionType.Bounce).SetDelay(0.2f);
        
        // 颜色变化效果
        _tween.TweenProperty(_label, "modulate", new Color(1.0f, 0.0f, 0.0f, 1.0f), 0.2f); // 从红色开始
        _tween.TweenProperty(_label, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.3f).SetDelay(0.2f); // 变为白色
        
        // //旋转效果
        // _tween.TweenProperty(this, "rotation_degrees", 10.0f, 0.2f).SetTrans(Tween.TransitionType.Bounce);
        // _tween.TweenProperty(this, "rotation_degrees", -10.0f, 0.2f).SetTrans(Tween.TransitionType.Bounce).SetDelay(0.2f);
        
        // 连接 Tween 的完成信号
        _tween.Connect("finished", new Callable(this, nameof(OnTweenCompleted)));
    }

    private void OnTweenCompleted()
    {
        // 当 Tween 完成时删除节点
        QueueFree();
    }

    public void SetDamage(int damage)
    {
        _label.Text = damage.ToString();
        
    }
}

