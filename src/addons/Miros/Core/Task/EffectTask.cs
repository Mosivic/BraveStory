using System;

namespace Miros.Core;

public class EffectTask(Effect effect) : TaskBase(effect)
{
    public event Action<EffectTask> OnStacked;
    public event Action<EffectTask> OnStackOverflowed;
    public event Action<EffectTask> OnDurationOvered;
    public event Action<EffectTask> OnPeriodOvered;


    public override void Enter()
    {
        base.Enter();
        CaptureAttributesSnapshot();

        // effect.Owner.TagAggregator.ApplyEffectDynamicTag(effect);
        effect.Owner.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);

        // TryActivateGrantedAbilities();

        if (effect.DurationPolicy == DurationPolicy.Instant) effect.Owner.ApplyModFromInstantEffect(effect);
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
        // effect.Owner.TagAggregator.RestoreEffectDynamicTags(effect);
        // TryDeactivateGrantedAbilities();
    }


    public override void Stack()
    {
        var stackingComponent = GetComponent<StackingComponent>();
        if (effect.DurationPolicy == DurationPolicy.Instant)
        {
        }

        // // Check GE Stacking
        // if (stackingComponent.StackingType == StackingType.None)
        // {
        //     return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
        // }

        // // 处理GE堆叠
        // // 基于Target类型GE堆叠
        // if (stackingComponent.StackingType == StackingType.AggregateByTarget)
        // {
        //     GetStackingEffectByData(effect, out var ge);
        //     // 新添加GE
        //     if (ge == null)
        //         return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
        //     bool stackCountChange = ge.RefreshStack();
        //     if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
        //     return ge;
        // }

        // // 基于Source类型GE堆叠
        // if (stackingComponent.StackingType == StackingType.AggregateBySource)
        // {
        //     GetStackingEffectByDataFrom(effect, source, out var ge);
        //     if (ge == null)
        //         return Operation_AddNewGameplayEffectSpec(source, effect, overwriteEffectLevel, effectLevel);
        //     bool stackCountChange = ge.RefreshStack();
        //     if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
        //     return ge;
        // }
    }


    public override bool CanEnter()
    {
        return effect.Owner.HasAllTags(effect.ApplicationRequiredTags);
    }


    public override bool CanExit()
    {
        return effect.Owner.HasAllTags(effect.OngoingRequiredTags) ||
               effect.Owner.HasAnyTags(effect.ApplicationImmunityTags);
    }


    // 捕获属性快照
    private void CaptureAttributesSnapshot()
    {
        effect.SnapshotSourceAttributes = effect.Source.DataSnapshot();
        effect.SnapshotTargetAttributes = effect.Source == effect.Owner
            ? effect.SnapshotSourceAttributes
            : effect.Owner.DataSnapshot();
    }


    public void RegisterOnStackCountChanged(Action<int, int> callback)
    {
        effect.OnStackCountChanged += callback;
    }

    public void UnregisterOnStackCountChanged(Action<int, int> callback)
    {
        effect.OnStackCountChanged -= callback;
    }


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