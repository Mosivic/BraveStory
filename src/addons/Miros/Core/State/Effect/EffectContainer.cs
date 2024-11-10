using System;
using System.Collections.Generic;

namespace Miros.Core;

public class EffectContainer
{
    private readonly Persona _owner;
    private readonly List<Effect> _effects = new List<Effect>();
    private readonly List<Effect> _cachedEffects = new List<Effect>();

    public EffectContainer(Persona owner)
    {
        _owner = owner;
    }

    private event Action OnGameplayEffectContainerIsDirty;

    public List<Effect> Effects()
    {
        return _effects;
    }

    public void Tick()
    {
        _cachedEffects.AddRange(_effects);

        foreach (var effect in _cachedEffects)
        {
            if (effect.IsActive)
            {
                effect.Tick();
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
            var assetTags = effect.AssetTags;
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


    public void RemoveEffect(Effect effect)
    {
        _effects.Remove(effect);
        effect.DisApply();
        effect.TriggerOnRemove();
    }

    /// <summary>
    /// 添加GE
    /// </summary>
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
        if (effect.Stacking.stackingType == StackingType.None)
        {
            return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
        }

        // 处理GE堆叠
        // 基于Target类型GE堆叠
        if (effect.Stacking.stackingType == StackingType.AggregateByTarget)
        {
            GetStackingEffectSpecByData(effect, out var geSpec);
            // 新添加GE
            if (geSpec == null)
                return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
            bool stackCountChange = geSpec.RefreshStack();
            if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            return geSpec;
        }

        // 基于Source类型GE堆叠
        if (effect.Stacking.stackingType == StackingType.AggregateBySource)
        {
            GetStackingEffectSpecByDataFrom(effect, source, out var geSpec);
            if (geSpec == null)
                return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
            bool stackCountChange = geSpec.RefreshStack();
            if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
            return geSpec;
        }

        return null;
    }


    public void RefreshGameplayEffectState()
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

    private void GetStackingEffectSpecByData(Effect effect, out Effect spec)
    {
        foreach (var gameplayEffectSpec in _effects)
            if (gameplayEffectSpec.GameplayEffect.StackEqual(effect))
            {
                spec = gameplayEffectSpec;
                return;
            }

        spec = null;
    }

    private void GetStackingEffectSpecByDataFrom(GameplayEffect effect, AbilitySystemComponent source,
        out GameplayEffectSpec spec)
    {
        foreach (var gameplayEffectSpec in _effects)
            if (gameplayEffectSpec.Source == source &&
                gameplayEffectSpec.GameplayEffect.StackEqual(effect))
            {
                spec = gameplayEffectSpec;
                return;
            }

        spec = null;
    }

    private void OnRefreshStackCountMakeContainerDirty()
    {
        OnGameplayEffectContainerIsDirty?.Invoke();
    }

}
