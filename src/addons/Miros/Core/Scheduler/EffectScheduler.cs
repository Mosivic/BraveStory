using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class EffectScheduler(Persona owner):SchedulerBase<EffectJob>
{
    private readonly Persona _owner = owner;

    private event Action OnEffectsIsDirty;


    public override void Update(double delta)
    {
        
    }

    public void RegisterOnEffectsIsDirty(Action action)
    {
        OnEffectsIsDirty += action;
    }

    public void UnregisterOnEffectsIsDirty(Action action)
    {
        OnEffectsIsDirty -= action;
    }



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

        OnEffectsIsDirty?.Invoke();
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
        OnEffectsIsDirty?.Invoke();
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

        OnEffectsIsDirty?.Invoke();
        return effectSpec;
    }

}
