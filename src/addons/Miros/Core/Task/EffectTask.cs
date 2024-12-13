using System;
# if GODOT
using Godot;
# endif

namespace Miros.Core;

public class EffectTask(Effect effect) : TaskBase(effect)
{
    private EffectUpdateHandler _updateHandler;

    public bool IsInstant => effect.DurationPolicy == DurationPolicy.Instant;
    public EffectStacking Stacking => effect.Stacking;



    public override void Enter()
    {
        base.Enter();
        CaptureAttributesSnapshot();

        effect.OwnerAgent.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);


        if (effect.DurationPolicy == DurationPolicy.Instant)
        {
            effect.OwnerAgent.ApplyModWithInstant(effect);
            effect.OwnerAgent.ApplyExecWithInstant(effect);

            effect.Status = RunningStatus.Succeed;
        }
        else if (effect.DurationPolicy == DurationPolicy.Infinite)
        {
        }
        else if (effect.DurationPolicy == DurationPolicy.Duration || effect.DurationPolicy == DurationPolicy.Periodic)
        {
            _updateHandler = new EffectUpdateHandler(effect);
        }
    }


    public override void Update(double delta)
    {
        base.Update(delta);
        _updateHandler?.Tick(delta);
    }


    public override void Exit()
    {
        base.Exit();
    }


    public void Stack(bool isFromSameSource = false) //调用该方法时已经确保 StackingComponent 存在
    {
        var stacking = effect.Stacking;

        switch (stacking.StackingType)
        {
            case StackingType.None:
                break;
            case StackingType.AggregateBySource:
                if (isFromSameSource)
# if GODOT && DEBUG
                    GD.Print(
                        $"[EffectTask][{effect.Tag.ShortName}] StackCount changed from {stacking.StackCount} to {stacking.StackCount + 1}");
# endif
                stacking.ChangeStackCount(stacking.StackCount + 1);
                break;
            case StackingType.AggregateByTarget:

# if GODOT && DEBUG
                GD.Print(
                    $"[EffectTask][{effect.Tag.ShortName}] StackCount changed from {stacking.StackCount} to {stacking.StackCount + 1}");
# endif
                stacking.ChangeStackCount(stacking.StackCount + 1);
                break;
            default:
                throw new ArgumentException($"Invalid StackingType: {stacking.StackingType}");
        }
    }


    public override bool CanEnter()
    {
        return effect.OwnerAgent.HasAll(effect.ApplicationRequiredTags);
    }


    public override bool CanExit()
    {
        if (!effect.OwnerAgent.HasAll(effect.OngoingRequiredTags)) return true;
        if (effect.OwnerAgent.HasAny(effect.ApplicationImmunityTags)) return true;
        return effect.Status != RunningStatus.Running;
    }

    // 捕获属性快照
    private void CaptureAttributesSnapshot()
    {
        effect.SnapshotSourceAttributes = effect.SourceAgent.DataSnapshot();
        effect.SnapshotTargetAttributes = effect.SourceAgent == effect.OwnerAgent
            ? effect.SnapshotSourceAttributes
            : effect.OwnerAgent.DataSnapshot();
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