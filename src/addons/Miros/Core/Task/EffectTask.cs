using System;
using Godot;

namespace Miros.Core;

public class EffectTask(Effect effect) : TaskBase(effect)
{
    public event Action<EffectTask> OnStacked;
    public event Action<EffectTask> OnStackOverflowed;
    public event Action<EffectTask> OnDurationOvered;
    public event Action<EffectTask> OnPeriodOvered;

    public bool IsInstant => effect.DurationPolicy == DurationPolicy.Instant;
    private EffectPeriodTicker _periodTicker;

    public override void Enter()
    {
        base.Enter();
        CaptureAttributesSnapshot();

        // effect.Owner.TagAggregator.ApplyEffectDynamicTag(effect);
        effect.Owner.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);

        // TryActivateGrantedAbilities();

        if (effect.DurationPolicy == DurationPolicy.Instant)
        {
            effect.Owner.ApplyModFromInstantEffect(effect);
            effect.Status = RunningStatus.Succeed;
        }
        else
        {
            _periodTicker = new EffectPeriodTicker(effect);
        }
    }


    public override void Update(double delta)
    {
        base.Update(delta);
        _periodTicker.Tick(delta);
    }


    public override void Exit()
    {
        base.Exit();

        // TryRemoveGrantedAbilities();
        // effect.Owner.TagAggregator.RestoreEffectDynamicTags(effect);
        // TryDeactivateGrantedAbilities();
    }


    public  void Stack()
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
        return effect.Owner.HasAll(effect.ApplicationRequiredTags);
    }


    public override bool CanExit()
    {
        if(!effect.Owner.HasAll(effect.OngoingRequiredTags)) return true;
        if(effect.Owner.HasAny(effect.ApplicationImmunityTags)) return true;
        return effect.Status != RunningStatus.Running;
    }


    // 捕获属性快照
    private void CaptureAttributesSnapshot()
    {
        effect.SnapshotSourceAttributes = effect.Source.DataSnapshot();
        effect.SnapshotTargetAttributes = effect.Source == effect.Owner
            ? effect.SnapshotSourceAttributes
            : effect.Owner.DataSnapshot();
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