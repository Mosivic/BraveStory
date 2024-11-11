using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class EffectContainer(Persona owner)
{
    private readonly Persona _owner = owner;
    private readonly List<Effect> _effects = [];
    private readonly List<Effect> _cachedEffects = [];
    public List<Effect> Effects => _effects;


    private event Action OnGameplayEffectContainerIsDirty;


    public void Tick(double delta)
    {
        _cachedEffects.AddRange(_effects);

        foreach (var effect in _cachedEffects)
        {
            if (effect.IsActive)
            {
                effect.Tick(delta);
            }
        }

        _cachedEffects.Clear();
    }

    /// <summary>
    /// 注册GE容器变化事件
    /// </summary>
    /// <param name="action"></param>
    public void RegisterOnEffectContainerIsDirty(Action action)
    {
        OnGameplayEffectContainerIsDirty += action;
    }

    public void UnregisterOnEffectContainerIsDirty(Action action)
    {
        OnGameplayEffectContainerIsDirty -= action;
    }

    /// <summary>
    /// 移除包含任意指定标签的GE
    /// </summary>
    /// <param name="tags"></param>
    public void RemoveEffectWithAnyTags(TagSet tags)
    {
        if (tags.Empty) return;

        var removeList = new List<Effect>();
        foreach (var effect in _effects)
        {
            var assetTags = effect.OwnedTags;
            if (!assetTags.Empty && assetTags.HasAnyTags(tags))
            {
                removeList.Add(effect);
                continue;
            }

            var grantedTags = effect.GrantedTags;
            if (!grantedTags.Empty && grantedTags.HasAnyTags(tags)) removeList.Add(effect);
        }

        foreach (var effect in removeList) RemoveEffect(effect);
    }


    // public void RemoveEffect(Effect effect)
    // {
    //     _effects.Remove(effect);
    //     effect.DisApply();
    //     effect.TriggerOnRemove();
    // }


    public Effect AddEffect(Persona source, Effect effect, bool overwriteEffectLevel = false, int effectLevel = 0)
    {
        if (!effect.CanApplyTo(_owner)) return null;

        if (effect.IsImmune(_owner))
        {
            // TODO 免疫Cue触发
            // var lv = overwriteEffectLevel ? effectLevel : source.Level;
            // effectSpec.Init(source, _owner, lv);
            // effectSpec.TriggerOnImmunity();
            return null;
        }

        var level = overwriteEffectLevel ? effectLevel : source.Level;
        if (effect.DurationPolicy == DurationPolicy.Instant)
        {
            effect.Init(source, _owner, level);
            effect.TriggerOnExecute();
            return null;
        }

        // Check GE Stacking
        if (effect.Stacking.StackingType == StackingType.None)
        {
            return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
        }

        // 处理GE堆叠
        // 基于Target类型GE堆叠
        if (effect.Stacking.StackingType == StackingType.AggregateByTarget)
        {
            GetStackingEffectByData(effect, out var ge);
            // 新添加GE
            if (ge == null)
                return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
            bool stackCountChange = ge.RefreshStack();
            if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            return ge;
        }

        // 基于Source类型GE堆叠
        if (effect.Stacking.StackingType == StackingType.AggregateBySource)
        {
            GetStackingEffectByDataFrom(effect, source, out var ge);
            if (ge == null)
                return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
            bool stackCountChange = ge.RefreshStack();
            if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            return ge;
        }

        return null;
    }


    public void RefreshEffectStatus()
    {
        foreach (var effect in _effects)
        {
            if (!effect.IsApplied) continue;
            if (!effect.IsActive)
            {
                // new active gameplay effects
                if (effect.CanRunning(_owner)) effect.Activate();
            }
            else
            {
                // new deactive gameplay effects
                if (!effect.CanRunning(_owner)) effect.Deactivate();
            }
        }

        OnGameplayEffectContainerIsDirty?.Invoke();
    }

    /// <summary>
    /// 检查GE冷却
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public CooldownTimer CheckCooldownFromTags(TagSet tags)
    {
        float longestCooldown = 0;
        float maxDuration = 0;

        // Check if the cooldown tag is granted to the player, and if so, capture the remaining duration for that tag
        foreach (var effect in _effects)
        {
            if (effect.IsActive)
            {
                var grantedTags = effect.GrantedTags;
                if (grantedTags.Empty) continue;
                foreach (var t in grantedTags.Tags)
                    foreach (var targetTag in tags.Tags)
                    {
                        if (t != targetTag) continue;
                        // If this is an infinite GE, then return null to signify this is on CD
                        if (effect.DurationPolicy ==
                            DurationPolicy.Infinite)
                            return new CooldownTimer { TimeRemaining = -1, Duration = 0 };

                        var durationRemaining = effect.DurationRemaining();

                        if (!(durationRemaining > longestCooldown)) continue;
                        longestCooldown = durationRemaining;
                        maxDuration = effect.Duration;
                    }
            }
        }
        return new CooldownTimer { TimeRemaining = longestCooldown, Duration = maxDuration };
    }

    public void ClearEffect()
    {
        foreach (var effect in _effects)
        {
            effect.DisApply();
            effect.TriggerOnRemove();
        }

        _effects.Clear();

        OnGameplayEffectContainerIsDirty?.Invoke();
    }

    private void GetStackingEffectByData(Effect effect, out Effect ge)
    {
        foreach (var _effect in _effects)
            if (_effect.StackEqual(effect))
            {
                ge = _effect;
                return;
            }
        ge = null;
    }

    private void GetStackingEffectByDataFrom(Effect effect, Persona source,
        out Effect ge)
    {
        foreach (var _effect in _effects)
            if (_effect.Source == source &&
                _effect.StackEqual(effect))
            {
                ge = _effect;
                return;
            }
        ge = null;
    }

    private void OnRefreshStackCountMakeContainerDirty()
    {
        OnGameplayEffectContainerIsDirty?.Invoke();
    }


    private Effect Operation_AddNewGameplayEffectSpec(Persona source, Effect effectSpec,
    bool overwriteEffectLevel, int effectLevel)
    {
        var level = overwriteEffectLevel ? effectLevel : source.Level;
        effectSpec.Init(source, _owner, level);
        _effects.Add(effectSpec);
        effectSpec.TriggerOnAdd();
        effectSpec.Apply();

        // If the gameplay effect was removed immediately after being applied, return false
        if (!_effects.Contains(effectSpec))
        {
#if GODOT4
            GD.Print($"Effect {effectSpec.Name} was removed immediately after being applied. This may indicate a problem with the RemoveEffectWithAnyTags.");
#endif
            // No need to trigger OnGameplayEffectContainerIsDirty, it has already been triggered when it was removed.
            return null;
        }

        OnGameplayEffectContainerIsDirty?.Invoke();
        return effectSpec;
    }

}
