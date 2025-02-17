using System;
# if GODOT
using Godot;
# endif

namespace Miros.Core;

public class EffectTask : TaskBase<Effect>
{
    private EffectUpdateHandler _updateHandler;


    public override void Enter(State state)
    {
        base.Enter(state);
        var effect = state as Effect;
        CaptureAttributesSnapshot(effect);

        effect.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);


        if (effect.DurationPolicy == DurationPolicy.Instant)
        {
            effect.ApplyModWithInstant();

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


    public override void Update(State state, double delta)
    {
        base.Update(state, delta);
        _updateHandler?.Tick(delta);
    }


    public override void Exit(State effect)
    {
        base.Exit(effect);
    }


    public override void Stack(State state, bool isFromSameSource = false) //调用该方法时已经确保 StackingComponent 存在
    {
        var effect = state as Effect;
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


    public override bool CanEnter(State state)
    {
        var effect = state as Effect;
        return effect.OwnerAgent.HasAll(effect.ApplicationRequiredTags);
    }


    public override bool CanExit(State state)
    {
        var effect = state as Effect;
        if (!effect.OwnerAgent.HasAll(effect.OngoingRequiredTags)) return true;
        if (effect.OwnerAgent.HasAny(effect.ApplicationImmunityTags)) return true;
        return effect.Status != RunningStatus.Running;
    }

    // 捕获属性快照
    private void CaptureAttributesSnapshot(Effect effect)
    {
        effect.SnapshotSourceAttributes = effect.SourceAgent.DataSnapshot();
        effect.SnapshotTargetAttributes = effect.SourceAgent == effect.OwnerAgent
            ? effect.SnapshotSourceAttributes
            : effect.OwnerAgent.DataSnapshot();
    }
}