using Godot;
using Miros.Core;

namespace BraveStory;

public partial class StatsPanel : Control
{
	[Export]
	public Character Character;
	private Agent _agent;

	private TextureProgressBar _healthBar;
	private TextureProgressBar _easeHealthBar;

	// TODO: 添加耐力条
	private TextureProgressBar _staminaBar;



	public override void _Ready()
	{
		base._Ready();

		_agent = Character.Agent;
		_agent.GetAttributeBase("HP").RegisterPostCurrentValueChange(OnUpdateHealthBar);

		_healthBar = GetNode<TextureProgressBar>("HealthBar");
		_easeHealthBar = GetNode<TextureProgressBar>("HealthBar/EaseHealthBar");
		_staminaBar = GetNode<TextureProgressBar>("StaminaBar");
	}


	private void OnUpdateHealthBar(AttributeBase attr, float oldValue, float newValue)
	{
		var percent = newValue / attr.MaxValue;
		_healthBar.Value = percent;

		CreateTween().TweenProperty(_easeHealthBar, "value", percent, 0.5f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
	}

}
