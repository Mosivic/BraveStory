using Godot;
namespace Miros.Core;

public class BurningTagHandler : TagEventHandler
{
	private float _damagePerSecond = 10f;
	private float _damageAccumulator = 0f;
	
	public BurningTagHandler() 
		: base(TagManager.Instance.RequestGameplayTag("Status.Burning"))
	{
	}
	
	public override void OnTagAdded(TagContainer container, Node owner)
	{
		// 开始燃烧效果
		if (owner is CharacterBody2D character)
		{
		   // character.PlayParticleEffect("fire");
		}
	}
	
	public override void OnTagUpdate(TagContainer container, Node owner, double delta)
	{
		// 持续伤害
		if (owner is CharacterBody2D character)
		{
			_damageAccumulator += (float)delta;
			if (_damageAccumulator >= 1.0f)
			{
				//character.TakeDamage(_damagePerSecond);
				_damageAccumulator -= 1.0f;
			}
		}
	}
	
	public override void OnTagRemoved(TagContainer container, Node owner)
	{
		// 停止燃烧效果
		if (owner is CharacterBody2D character)
		{
			//character.StopParticleEffect("fire");
		}
	}
}
