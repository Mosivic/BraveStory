using System;
using System.Collections.Generic;

namespace Miros.Core;

public class AttributeAggregator(AttributeBase attribute, Agent owner)
{
    private readonly AttributeBase _attribute = attribute;
    private readonly Dictionary<Modifier, Effect> _dirctModifierCache = [];
    private readonly Dictionary<Modifier,Effect> _postModifierCache = [];
    private EffectExecutor _effectExecutor;


    public void Enable()
    {
        _effectExecutor = owner.GetExecutor(ExecutorType.EffectExecutor) as EffectExecutor;
        // 注册基础值变化事件
        _attribute.RegisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueChanged);
        // 注册游戏效果容器变化事件
        _effectExecutor.RegisterOnRunningEffectTasksIsDirty(RefreshModifierCache);
    }

    public void Disable()
    {
        // 注销基础值变化事件
        _attribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueChanged);
        // 注销游戏效果容器变化事件
        _effectExecutor.UnregisterOnRunningEffectTasksIsDirty(RefreshModifierCache);
    }


    /// <summary>
    ///     刷新修改器缓存
    ///     当游戏效果被添加或移除时触发
    /// </summary>
    private void RefreshModifierCache(Effect effect, bool isAdd)
    {
        if (isAdd)
        {
            foreach (var modifier in effect.Modifiers)
                if (modifier.AttributeTag == _attribute.AttributeTag)
                {
                    if(modifier.Type == ModifierType.Direct)
                    {
                        _dirctModifierCache.Add(modifier, effect);
                        TryRegisterAttributeChangedListen(effect, modifier);
                    }
                    else if(modifier.Type == ModifierType.Post)
                    {
                        _postModifierCache.Add(modifier,effect);
                    }
                }
        }
        else
        {
            foreach (var keyValuePair in _dirctModifierCache)
                TryUnregisterAttributeChangedListen(keyValuePair.Value, keyValuePair.Key);

            _dirctModifierCache.Clear();
            _postModifierCache.Clear();
        }

        UpdateCurrentValueWhenModifierIsDirty();
    }

    

    /// <summary>
    ///     计算新值
    /// </summary>
    /// <returns></returns>
    private float CalculateNewValue()
    {
        switch (_attribute.CalculateMode)
        {
            case CalculateMode.Stacking:
            {
                var newValue = _attribute.BaseValue;
                foreach (var keyValuePair in _dirctModifierCache)
                {
                    var effect = keyValuePair.Value;
                    var modifier = keyValuePair.Key;
                    var magnitude = modifier.CalculateMagnitude(effect);

                    if (!_attribute.IsSupportOperation(modifier.Operation))
                        throw new InvalidOperationException("Unsupported operation.");

                    newValue = CaculateModifierValue(newValue, effect, modifier);
                }

                return newValue;
            }
            case CalculateMode.MinValueOnly:
            {
                var hasOverride = false;
                var min = float.MaxValue;
                foreach (var keyValuePair in _dirctModifierCache)
                {
                    var effect = keyValuePair.Value;
                    var modifier = keyValuePair.Key;

                    if (!_attribute.IsSupportOperation(modifier.Operation))
                        throw new InvalidOperationException("Unsupported operation.");

                    if (modifier.Operation != ModifierOperation.Override)
                        throw new InvalidOperationException("MinValueOnly mode only supports override operation.");

                    var magnitude = modifier.CalculateMagnitude(effect);
                    min = Math.Min(min, magnitude);
                    hasOverride = true;
                }

                return hasOverride ? min : _attribute.BaseValue;
            }
            case CalculateMode.MaxValueOnly:
            {
                var hasOverride = false;
                var max = float.MinValue;
                foreach (var keyValuePair in _dirctModifierCache)
                {
                    var effect = keyValuePair.Value;
                    var modifier = keyValuePair.Key;

                    if (!_attribute.IsSupportOperation(modifier.Operation))
                        throw new InvalidOperationException("Unsupported operation.");

                    if (modifier.Operation != ModifierOperation.Override)
                        throw new InvalidOperationException("MaxValueOnly mode only supports override operation.");

                    var magnitude = modifier.CalculateMagnitude(effect);
                    max = Math.Max(max, magnitude);
                    hasOverride = true;
                }

                return hasOverride ? max : _attribute.BaseValue;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

   // 作用是计算直接修改器和后置修改器对基础值的影响
    private float CaculateModifierValue(float baseValue, Effect effect, Modifier directModifier)
    {
        var directMagnitude = directModifier.CalculateMagnitude(effect);
        var newValue = ApplyOperation(directModifier.Operation, baseValue, directMagnitude);

        foreach (var keyValuePair in _postModifierCache)
        {
            var postModifier = keyValuePair.Key;
            var postEffect = keyValuePair.Value;

            if(postModifier.Operation == directModifier.Operation)
            {
                var postMagnitude = postModifier.CalculateMagnitude(postEffect);
                newValue = ApplyOperation(postModifier.PostOperation, newValue, postMagnitude);
            }
        }

        return newValue;
    }

    
    private static float ApplyOperation(ModifierOperation operation,float oldValue, float newValue)
    {
        switch (operation)
        {
            case ModifierOperation.Add:
                return oldValue + newValue;
            case ModifierOperation.Minus:
                return oldValue - newValue;
            case ModifierOperation.Multiply:
                return oldValue * newValue;
            case ModifierOperation.Divide:
                return oldValue / newValue;
            case ModifierOperation.Override:
                return newValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    ///     当基础值变化时更新当前值
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="oldBaseValue"></param>
    /// <param name="newBaseValue"></param>
    private void UpdateCurrentValueWhenBaseValueChanged(AttributeBase attribute, float oldBaseValue, float newBaseValue)
    {
        if (Math.Abs(oldBaseValue - newBaseValue) < float.Epsilon) return;

        var newValue = CalculateNewValue();
        _attribute.SetCurrentValue(newValue);
    }

    /// <summary>
    ///     当修改器变化时更新当前值
    /// </summary>
    private void UpdateCurrentValueWhenModifierIsDirty()
    {
        var newValue = CalculateNewValue();
        _attribute.SetCurrentValue(newValue);
    }


    /// <summary>
    ///     尝试注销属性变化事件
    /// </summary>
    /// <param name="ge"></param>
    /// <param name="modifier"></param>
    private void TryUnregisterAttributeChangedListen(Effect ge, Modifier modifier)
    {
        if (modifier.MMC is AttributeBasedModCalculation mmc &&
            mmc.captureType == AttributeBasedModCalculation.AttributeCaptureType.Track)
        {
            if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFromType.Target)
            {
                if (ge.OwnerAgent != null)
                    ge.OwnerAgent.GetAttributeBase(mmc.attributeBasedSetTag, mmc.attributeBasedTag)
                        .UnregisterPostCurrentValueChange(OnAttributeChanged);
            }
            else
            {
                if (ge.SourceAgent != null)
                    ge.SourceAgent.GetAttributeBase(mmc.attributeBasedSetTag, mmc.attributeBasedTag)
                        .UnregisterPostCurrentValueChange(OnAttributeChanged);
            }
        }
    }

    /// <summary>
    ///     注册属性变化事件
    /// </summary>
    /// <param name="ge">游戏效果</param>
    /// <param name="modifier">修改器</param>
    private void TryRegisterAttributeChangedListen(Effect ge, Modifier modifier)
    {
        if (modifier.MMC is AttributeBasedModCalculation mmc &&
            mmc.captureType == AttributeBasedModCalculation.AttributeCaptureType.Track)
        {
            if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFromType.Target)
            {
                if (ge.OwnerAgent != null)
                    ge.OwnerAgent.GetAttributeBase(mmc.attributeBasedSetTag, mmc.attributeBasedTag)
                        .RegisterPostCurrentValueChange(OnAttributeChanged);
            }
            else
            {
                if (ge.SourceAgent != null)
                    ge.SourceAgent.GetAttributeBase(mmc.attributeBasedSetTag, mmc.attributeBasedTag)
                        .RegisterPostCurrentValueChange(OnAttributeChanged);
            }
        }
    }


    /// <summary>
    ///     属性变化事件
    /// </summary>
    /// <param name="attribute">属性</param>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    private void OnAttributeChanged(AttributeBase attribute, float oldValue, float newValue)
    {
        if (_dirctModifierCache.Count == 0) return;
        foreach (var keyValuePair in _dirctModifierCache)
        {
            var effect = keyValuePair.Value;
            var modifier = keyValuePair.Key;
            if (modifier.MMC is AttributeBasedModCalculation mmc &&
                mmc.captureType == AttributeBasedModCalculation.AttributeCaptureType.Track &&
                attribute.AttributeTag == mmc.attributeBasedTag)
                if ((mmc.attributeFromType == AttributeBasedModCalculation.AttributeFromType.Target &&
                     attribute.Owner == effect.OwnerAgent) ||
                    (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFromType.Source &&
                     attribute.Owner == effect.SourceAgent))
                {
                    UpdateCurrentValueWhenModifierIsDirty();
                    break;
                }
        }
    }
}