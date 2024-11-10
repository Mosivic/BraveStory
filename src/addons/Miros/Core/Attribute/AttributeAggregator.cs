// 这是一个属性聚合器类，负责管理和计算游戏中属性的最终值。
// 1. 核心功能
// 这个类的主要职责是管理属性的计算过程，包括基础值和各种修改器的应用。
// 2. 重要字段
// 3. 主要功能模块
// 3.1 生命周期管理
// - 注册/注销属性基础值变化的监听
// 注册/注销游戏效果容器变化的监听
// 3.2 修改器缓存刷新
// 当游戏效果被添加或移除时触发，负责：
// 清理旧的修改器缓存
// 重新收集所有活跃的修改器
// 注册必要的属性变化监听
// 3.3 属性值计算
// 支持三种计算模式：
// Stacking（堆叠模式）
// 从基础值开始
// 按顺序应用所有修改器（加、减、乘、除、覆盖）
// MinValueOnly（最小值模式）
// 只支持覆盖操作
// 取所有修改器中的最小值
// MaxValueOnly（最大值模式）
// 只支持覆盖操作
// 取所有修改器中的最大值
// 3.4 属性变化监听
// - 处理基于属性的修改器计算（AttributeBased MMC）
// 跟踪其他属性的变化并更新当前属性值
// 4. 触发机制
// 属性值的重新计算会在以下情况触发：
// 1. 修改器缓存发生变化时
// 属性的基础值发生变化时
// 3. 被跟踪的其他属性值发生变化时
// 5. 设计特点
// 模块化：清晰的职责划分
// 灵活性：支持多种计算模式
// 响应式：自动响应相关属性的变化
// 安全性：包含异常处理和验证
// 6. 使用场景
// 这个类通常用于：
// 角色属性系统（如生命值、攻击力等）
// buff/debuff 系统
// 装备加成系统
// 技能效果系统
// 7. 注意事项
// 修改器的执行顺序很重要，会影响最终计算结果
// 不同计算模式有不同的操作限制
// 需要正确管理监听器的注册和注销
// 属性间的相互依赖需要谨慎处理，避免循环依赖
// 8. 工作流程示例
// 创建属性聚合器实例
// 注册必要的监听器
// 添加游戏效果和修改器
// 4. 自动计算并更新属性值
// 响应各种变化并重新计算
// 这个类是一个复杂的属性系统的核心组件，提供了强大的属性计算和管理功能，类似于虚幻引擎的 GAS 系统。它能够处理各种复杂的属性计算场景，为游戏中的属性系统提供了可靠的基础设施。

using System;
using System.Collections.Generic;

namespace Miros.Core;

public class AttributeAggregator(AttributeBase attribute, Persona owner)
{
    private readonly Persona _owner = owner;
    private readonly AttributeBase _processedAttribute = attribute;
    private readonly List<Tuple<Effect, Modifier>> _modifierCache = [];

    public void OnEnable()
    {
        // 注册基础值变化事件
        _processedAttribute.RegisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
        // 注册游戏效果容器变化事件
        _owner.EffectContainer.RegisterOnEffectContainerIsDirty(RefreshModifierCache);
    }

    public void OnDisable()
    {
        // 注销基础值变化事件
        _processedAttribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
        // 注销游戏效果容器变化事件
        _owner.EffectContainer.UnregisterOnEffectContainerIsDirty(RefreshModifierCache);
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

        var effects = _owner.EffectContainer.Effects();
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
