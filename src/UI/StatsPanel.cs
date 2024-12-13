using Godot;
using Miros.Core;

namespace BraveStory;

public partial class StatsPanel : Control
{
    private TextureProgressBar _easeHealthBar;
    private TextureProgressBar _healthBar;

    // TODO: 添加耐力条
    private TextureProgressBar _staminaBar;

    public override void _Ready()
    {
        base._Ready();

        _healthBar = GetNode<TextureProgressBar>("HealthBar");
        _easeHealthBar = GetNode<TextureProgressBar>("HealthBar/EaseHealthBar");
        //_staminaBar = GetNode<TextureProgressBar>("StaminaBar");
    }

    public void OnUpdateHealthBar(AttributeBase attr, float oldValue, float newValue)
    {
        var percent = newValue / attr.MaxValue;
        _healthBar.Value = percent;

        CreateTween().TweenProperty(_easeHealthBar, "value", percent, 0.5f).SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
    }
}