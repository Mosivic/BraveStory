using System;
using System.Collections.Generic;

namespace Miros.Core;

public class AttributeAggregator(AttributeBase attribute,Persona owner)
{
	private readonly AttributeBase _processedAttribute = attribute;
	private readonly List<Tuple<Effect, Modifier>> _modifierCache = [];

	public void OnEnable()
	{
		// 注册基础值变化事件
		_processedAttribute.RegisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
		// 注册游戏效果容器变化事件
		owner.GetEffectScheduler()?.RegisterOnEffectsIsDirty(RefreshModifierCache);
	}
	
	public void OnDisable()
	{
		// 注销基础值变化事件
		_processedAttribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
		// 注销游戏效果容器变化事件
		owner.GetEffectScheduler()?.UnregisterOnEffectsIsDirty(RefreshModifierCache);
	}

	/// <summary>
	/// 刷新修改器缓存
	/// 当游戏效果被添加或移除时触发
	/// </summary>
	void RefreshModifierCache()
	{
		// 注销属性变化事件
		UnregisterAttributeChangedListen();
		_modifierCache.Clear();

		var effects = owner.GetEffects();
		foreach (var ge in effects)
		{
			if (ge.IsActive)
			{
				foreach (var modifier in ge.Modifiers)
				{
					if (modifier.AttributeName == _processedAttribute.Name)
					{
						_modifierCache.Add(new Tuple<Effect, Modifier>(ge, modifier));
						TryRegisterAttributeChangedListen(ge, modifier);
					}
				}
			}
		}
		UpdateCurrentValueWhenModifierIsDirty();
	}

	/// <summary>
	/// 计算新值
	/// </summary>
	/// <returns></returns>
	float CalculateNewValue()
	{
		switch (_processedAttribute.CalculateMode)
		{
			case CalculateMode.Stacking:
				{
					float newValue = _processedAttribute.BaseValue;
					foreach (var tuple in _modifierCache)
					{
						var spec = tuple.Item1;
						var modifier = tuple.Item2;
						var magnitude = modifier.CalculateMagnitude(spec, modifier.Magnitude);

						if (!_processedAttribute.IsSupportOperation(modifier.Operation))
						{
							throw new InvalidOperationException("Unsupported operation.");
						}

						switch (modifier.Operation)
						{
							case ModifierOperation.Add:
								newValue += magnitude;
								break;
							case ModifierOperation.Minus:
								newValue -= magnitude;
								break;
							case ModifierOperation.Multiply:
								newValue *= magnitude;
								break;
							case ModifierOperation.Divide:
								newValue /= magnitude;
								break;
							case ModifierOperation.Override:
								newValue = magnitude;
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}

					return newValue;
				}
			case CalculateMode.MinValueOnly:
				{
					var hasOverride = false;
					var min = float.MaxValue;
					foreach (var tuple in _modifierCache)
					{
						var spec = tuple.Item1;
						var modifier = tuple.Item2;

						if (!_processedAttribute.IsSupportOperation(modifier.Operation))
						{
							throw new InvalidOperationException("Unsupported operation.");
						}

						if (modifier.Operation != ModifierOperation.Override)
						{
							throw new InvalidOperationException("MinValueOnly mode only supports override operation.");
						}

						var magnitude = modifier.CalculateMagnitude(spec, modifier.Magnitude);
						min = Math.Min(min, magnitude);
						hasOverride = true;
					}

					return hasOverride ? min : _processedAttribute.BaseValue;
				}
			case CalculateMode.MaxValueOnly:
				{
					var hasOverride = false;
					var max = float.MinValue;
					foreach (var tuple in _modifierCache)
					{
						var spec = tuple.Item1;
						var modifier = tuple.Item2;

						if (!_processedAttribute.IsSupportOperation(modifier.Operation))
						{
							throw new InvalidOperationException("Unsupported operation.");
						}

						if (modifier.Operation != ModifierOperation.Override)
						{
							throw new InvalidOperationException("MaxValueOnly mode only supports override operation.");
						}

						var magnitude = modifier.CalculateMagnitude(spec, modifier.Magnitude);
						max = Math.Max(max, magnitude);
						hasOverride = true;
					}

					return hasOverride ? max : _processedAttribute.BaseValue;
				}
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	/// <summary>
	/// 当基础值变化时更新当前值
	/// </summary>
	/// <param name="attribute"></param>
	/// <param name="oldBaseValue"></param>
	/// <param name="newBaseValue"></param>
	void UpdateCurrentValueWhenBaseValueIsDirty(AttributeBase attribute, float oldBaseValue, float newBaseValue)
	{
		if (Math.Abs(oldBaseValue - newBaseValue) < float.Epsilon) return;

		float newValue = CalculateNewValue();
		_processedAttribute.SetCurrentValue(newValue);
	}

	/// <summary>
	/// 当修改器变化时更新当前值
	/// </summary>
	void UpdateCurrentValueWhenModifierIsDirty()
	{
		float newValue = CalculateNewValue();
		_processedAttribute.SetCurrentValue(newValue);
	}


	/// <summary>
	/// 注销属性变化事件
	/// </summary>
	private void UnregisterAttributeChangedListen()
	{
		foreach (var tuple in _modifierCache)
			TryUnregisterAttributeChangedListen(tuple.Item1, tuple.Item2);
	}


	/// <summary>
	/// 尝试注销属性变化事件
	/// </summary>
	/// <param name="ge"></param>
	/// <param name="modifier"></param>
	private void TryUnregisterAttributeChangedListen(Effect ge, Modifier modifier)
	{
		if (modifier.MMC is AttributeBasedModCalculation mmc &&
			mmc.captureType == AttributeBasedModCalculation.EffectAttributeCaptureType.Track)
		{
			if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target)
			{
				if (ge.Owner != null)
					ge.Owner.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
						.UnregisterPostCurrentValueChange(OnAttributeChanged);
			}
			else
			{
				if (ge.Source != null)
					ge.Source.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
						.UnregisterPostCurrentValueChange(OnAttributeChanged);
			}
		}
	}

	/// <summary>
	/// 注册属性变化事件
	/// </summary>
	/// <param name="ge">游戏效果</param>
	/// <param name="modifier">修改器</param>
	private void TryRegisterAttributeChangedListen(Effect ge, Modifier modifier)
	{
		if (modifier.MMC is AttributeBasedModCalculation mmc &&
			mmc.captureType == AttributeBasedModCalculation.EffectAttributeCaptureType.Track)
		{
			if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target)
			{
				if (ge.Owner != null)
					ge.Owner.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
						.RegisterPostCurrentValueChange(OnAttributeChanged);
			}
			else
			{
				if (ge.Source != null)
					ge.Source.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
						.RegisterPostCurrentValueChange(OnAttributeChanged);
			}
		}
	}

	/// <summary>
	/// 属性变化事件    
	/// </summary>
	/// <param name="attribute">属性</param>
	/// <param name="oldValue">旧值</param>
	/// <param name="newValue">新值</param>
	private void OnAttributeChanged(AttributeBase attribute, float oldValue, float newValue)
	{
		if (_modifierCache.Count == 0) return;
		foreach (var tuple in _modifierCache)
		{
			var effect = tuple.Item1;
			var modifier = tuple.Item2;
			if (modifier.MMC is AttributeBasedModCalculation mmc &&
				mmc.captureType == AttributeBasedModCalculation.EffectAttributeCaptureType.Track &&
				attribute.Name == mmc.attributeName)
			{
				if ((mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target &&
					attribute.Owner == effect.Owner) ||
					(mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Source &&
					attribute.Owner == effect.Source))
				{
					UpdateCurrentValueWhenModifierIsDirty();
					break;
				}
			}
		}
	}
}
