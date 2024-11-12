using System;
using System.Collections.Generic;

namespace Miros.Core;

public class EffectJob(Effect effect) : JobBase(effect)
{
    public event Action<EffectJob> OnStacked;
    public event Action<EffectJob> OnStackOverflowed;
    public event Action<EffectJob> OnDurationOvered;
    public event Action<EffectJob> OnPeriodOvered;

    public bool CanStack => effect.Stacking.StackingType != StackingType.None;
    
    public override void Enter()
    {
        base.Enter();
        CaptureAttributesSnapshot();

        effect.Owner.TagAggregator.ApplyEffectDynamicTag(effect);
        effect.Owner.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);

        // TryActivateGrantedAbilities();
    
        if(effect.DurationPolicy == DurationPolicy.Instant)
        {
            effect.Owner.ApplyModFromInstantEffect(effect);
        }
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        effect.PeriodTicker.Tick(delta);
    }

    public override void Exit()
    {
        base.Exit();

        // TryRemoveGrantedAbilities();
        effect.Owner.TagAggregator.RestoreEffectDynamicTags(effect);
        // TryDeactivateGrantedAbilities();
    }

    public override bool CanEnter()
    {
        return effect.Owner.HasAllTags(effect.ApplicationRequiredTags);
    }

    public override bool CanExit()
    {
        return effect.Owner.HasAllTags(effect.OngoingRequiredTags) || effect.Owner.HasAnyTags(effect.ApplicationImmunityTags);
    }


    public void Stacking()
    {
        
    }

    // 捕获属性快照
    private void CaptureAttributesSnapshot()
    {
        effect.SnapshotSourceAttributes = effect.Source.DataSnapshot();
        effect.SnapshotTargetAttributes = effect.Source == effect.Owner ? effect.SnapshotSourceAttributes : effect.Owner.DataSnapshot();
    }


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



    // #region GrantedAbility
    // private void TryActivateGrantedAbilities()
    // {
    //     foreach (var grantedAbility in effect.GrantedAbility)
    //     {
    //         if (grantedAbility.ActivationPolicy == GrantedAbilityActivationPolicy.SyncWithEffect)
    //         {
    //             effect.Owner.TryActivateAbility(grantedAbility.AbilityName);
    //         }
    //     }
    // }

    // private void TryDeactivateGrantedAbilities()
    // {
    //     foreach (var grantedAbility in effect.GrantedAbility)
    //     {
    //         if (grantedAbility.DeactivationPolicy == GrantedAbilityDeactivationPolicy.SyncWithEffect)
    //         {
    //             effect.Owner.TryEndAbility(grantedAbility.AbilityName);
    //         }
    //     }
    // }

    // private void TryRemoveGrantedAbilities()
    // {
    //     foreach (var grantedAbility in effect.GrantedAbility)
    //     {
    //         if (grantedAbility.RemovePolicy == GrantedAbilityRemovePolicy.SyncWithEffect)
    //         {
    //             effect.Owner.TryCancelAbility(grantedAbility.AbilityName);
    //             effect.Owner.RemoveAbility(grantedAbility.AbilityName);
    //         }
    //     }
    // }
    // #endregion


}