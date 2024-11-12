using System;
using System.Collections.Generic;

namespace Miros.Core;

public class EffectJob(Effect effect) : JobBase(effect)
{
    public event Action<EffectJob> OnStacked;
    public event Action<EffectJob> OnStackOverflowed;
    public event Action<EffectJob> OnDurationOvered;
    public event Action<EffectJob> OnPeriodOvered;


    public override void Enter()
    {
        base.Enter();
        CaptureAttributesSnapshot();

        effect.Owner.TagAggregator.ApplyEffectDynamicTag(effect);
        effect.Owner.EffectContainer.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);

        TryActivateGrantedAbilities();
    }

    public override void Update(double delta)
    {
        if (effect.Status != RunningStatus.Running) return;
    
        effect.PeriodTicker.Tick(delta);
        base.Update(delta);
    }

    public override void Exit()
    {
        TryRemoveGrantedAbilities();
        effect.Owner.TagAggregator.RestoreEffectDynamicTags(effect);
        TryDeactivateGrantedAbilities();

        base.Exit();
    }



    // public bool CanApplyTo(Persona target)
    // {
    //     return target.HasAllTags(effect.ApplicationRequiredTags);
    // }

    // public bool CanRunning(Persona target)
    // {
    //     return target.HasAllTags(effect.OngoingRequiredTags);
    // }

    // public bool IsImmune(Persona target)
    // {
    //     return target.HasAnyTags(effect.ApplicationImmunityTags);
    // }



    // 捕获属性快照
    private void CaptureAttributesSnapshot()
    {
        effect.SnapshotSourceAttributes = effect.Source.DataSnapshot();
        effect.SnapshotTargetAttributes = effect.Source == effect.Owner ? effect.SnapshotSourceAttributes : effect.Owner.DataSnapshot();
    }

    // #region Value
    // public void RegisterValue(Tag tag, float value)
    // {
    //     effect.ValueMapWithTag[tag] = value;
    // }

    // public void RegisterValue(string name, float value)
    // {
    //     effect.ValueMapWithName[name] = value;
    // }

    // public bool UnregisterValue(Tag tag)
    // {
    //     return effect.ValueMapWithTag.Remove(tag);
    // }

    // public bool UnregisterValue(string name)
    // {
    //     return effect.ValueMapWithName.Remove(name);
    // }

    // public float? GetMapValue(Tag tag)
    // {
    //     return effect.ValueMapWithTag.TryGetValue(tag, out var value) ? value : (float?)null;
    // }

    // public float? GetMapValue(string name)
    // {
    //     return effect.ValueMapWithName.TryGetValue(name, out var value) ? value : (float?)null;
    // }
    // #endregion


    #region Stack
    public bool StackEqual(Effect anotherEffect)
    {
        if (effect.Stacking.StackingType == StackingType.None) return false;
        if (anotherEffect.Stacking.StackingType == StackingType.None) return false;
        if (string.IsNullOrEmpty(effect.Stacking.StackingCodeName)) return false;
        if (string.IsNullOrEmpty(anotherEffect.Stacking.StackingCodeName)) return false;

        return effect.Stacking.StackingHashCode == anotherEffect.Stacking.StackingHashCode;
    }

    public void RegisterOnStackCountChanged(Action<int, int> callback)
    {
        effect.OnStackCountChanged += callback;
    }

    public void UnregisterOnStackCountChanged(Action<int, int> callback)
    {
        effect.OnStackCountChanged -= callback;
    }
    #endregion



    #region Apply


    public void TriggerOnExecute()
    {
        effect.Owner.EffectContainer.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);
        effect.Owner.ApplyModFromInstantEffect(effect);

    }



    #endregion


    #region GrantedAbility
    private void TryActivateGrantedAbilities()
    {
        foreach (var grantedAbility in effect.GrantedAbility)
        {
            if (grantedAbility.ActivationPolicy == GrantedAbilityActivationPolicy.SyncWithEffect)
            {
                effect.Owner.TryActivateAbility(grantedAbility.AbilityName);
            }
        }
    }

    private void TryDeactivateGrantedAbilities()
    {
        foreach (var grantedAbility in effect.GrantedAbility)
        {
            if (grantedAbility.DeactivationPolicy == GrantedAbilityDeactivationPolicy.SyncWithEffect)
            {
                effect.Owner.TryEndAbility(grantedAbility.AbilityName);
            }
        }
    }

    private void TryRemoveGrantedAbilities()
    {
        foreach (var grantedAbility in effect.GrantedAbility)
        {
            if (grantedAbility.RemovePolicy == GrantedAbilityRemovePolicy.SyncWithEffect)
            {
                effect.Owner.TryCancelAbility(grantedAbility.AbilityName);
                effect.Owner.RemoveAbility(grantedAbility.AbilityName);
            }
        }
    }
    #endregion


}