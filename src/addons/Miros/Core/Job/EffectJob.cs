using System;

namespace Miros.Core;

public class EffectJob(Effect effect) : AbsJobBase(effect)
{

    public override void Enter()
    {
        effect.IsActive = true;
        TriggerOnActivation();

        CaptureAttributesSnapshot();

        if (effect.DurationPolicy != DurationPolicy.Instant)
        {
            if (effect.GrantedAbility.Length > 0)
            {
                TryActivateGrantedAbilities();
            }
        }


        base.Enter();
    }

    public override void Update(double delta)
    {
        if (effect.Status != RunningStatus.Running) return;
    
        effect.PeriodTicker.Tick(delta);
        base.Update(delta);
    }

    public override void Exit()
    {
        effect.IsActive = false;
        TriggerOnDeactivation();

        effect.CueDurational = null;


        DisApply();
        TriggerOnRemove();
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

    // public void RemoveSelf()
    // {
    //     effect.Owner.RemoveEffect(effect);
    // }

    private void CaptureAttributesSnapshot()
    {
        effect.SnapshotSourceAttributes = effect.Source.DataSnapshot();
        effect.SnapshotTargetAttributes = effect.Source == effect.Owner ? effect.SnapshotSourceAttributes : effect.Owner.DataSnapshot();
    }

    #region Value
    public void RegisterValue(Tag tag, float value)
    {
        effect.ValueMapWithTag[tag] = value;
    }

    public void RegisterValue(string name, float value)
    {
        effect.ValueMapWithName[name] = value;
    }

    public bool UnregisterValue(Tag tag)
    {
        return effect.ValueMapWithTag.Remove(tag);
    }

    public bool UnregisterValue(string name)
    {
        return effect.ValueMapWithName.Remove(name);
    }

    public float? GetMapValue(Tag tag)
    {
        return effect.ValueMapWithTag.TryGetValue(tag, out var value) ? value : (float?)null;
    }

    public float? GetMapValue(string name)
    {
        return effect.ValueMapWithName.TryGetValue(name, out var value) ? value : (float?)null;
    }
    #endregion


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
    public void Apply()
    {
        if (effect.IsApplied) return;
        effect.IsApplied = true;

        if (CanRunning(effect.Owner))
        {
            Activate();
        }
    }

    public void DisApply()
    {
        if (!effect.IsApplied) return;
        effect.IsApplied = false;
        Deactivate();
    }



    public void TriggerOnExecute()
    {
        effect.Owner.EffectContainer.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);
        effect.Owner.ApplyModFromInstantEffect(effect);

        TriggerCueOnExecute();
    }

    public void TriggerOnAdd()
    {
        TriggerCueOnAdd();
    }

    public void TriggerOnRemove()
    {
        TriggerCueOnRemove();
        TryRemoveGrantedAbilities();
    }

    private void TriggerOnActivation()
    {
        TriggerCueOnActivation();
        effect.Owner.TagAggregator.ApplyEffectDynamicTag(effect);
        effect.Owner.EffectContainer.RemoveEffectWithAnyTags(effect.RemoveEffectsWithTags);

        TryActivateGrantedAbilities();
    }

    private void TriggerOnDeactivation()
    {
        TriggerCueOnDeactivation();
        effect.Owner.TagAggregator.RestoreEffectDynamicTags(effect);

        TryDeactivateGrantedAbilities();
    }

    public void TriggerOnTick()
    {
        if (effect.DurationPolicy == DurationPolicy.Duration ||
            effect.DurationPolicy == DurationPolicy.Infinite)
            CueOnTick();
    }

    public void TriggerOnImmunity()
    {
        // TODO 免疫触发事件逻辑需要调整
        // onImmunity?.Invoke(Owner, this);
        // onImmunity = null;
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

    #region Cue
    private void TriggerInstantCues(Cue[] cues)
    {
        foreach (var cue in cues) cue.ApplyFrom(effect);
    }

    private void TriggerCueOnExecute()
    {
        if (effect.CueOnExecute == null || effect.CueOnExecute.Length <= 0) return;
        TriggerInstantCues(effect.CueOnExecute);
    }

    private void TriggerCueOnAdd()
    {
        if (effect.CueOnAdd != null && effect.CueOnAdd.Length > 0)
            TriggerInstantCues(effect.CueOnAdd);

        if (effect.CueDurational != null && effect.CueDurational.Length > 0)
        {
            effect.CueDurational.Clear();
            foreach (var cueDurational in effect.CueDurational)
            {
                var cueSpec = (CueDurational)cueDurational.ApplyFrom(effect);
                if (cueSpec != null) effect.CueDurational.Add(cueSpec);
            }

            foreach (var cue in effect.CueDurational) cue.OnAdd();
        }
    }

    private void TriggerCueOnRemove()
    {
        if (effect.CueOnRemove != null && effect.CueOnRemove.Length > 0)
            TriggerInstantCues(effect.CueOnRemove);

        if (effect.CueDurational != null && effect.CueDurational.Length > 0)
        {
            foreach (var cue in effect.CueDurational) cue.OnRemove();

            effect.CueDurational = null;
        }
    }

    private void TriggerCueOnActivation()
    {
        if (effect.CueOnActivate != null && effect.CueOnActivate.Length > 0)
            TriggerInstantCues(effect.CueOnActivate);

        if (effect.CueDurational != null && effect.CueDurational.Length > 0)
            foreach (var cue in effect.CueDurational)
                cue.OnGameplayEffectActivate();
    }

    private void TriggerCueOnDeactivation()
    {
        if (effect.CueOnDeactivate != null && effect.CueOnDeactivate.Length > 0)
            TriggerInstantCues(effect.CueOnDeactivate);

        if (effect.CueDurational != null && effect.CueDurational.Length > 0)
            foreach (var cue in effect.CueDurational)
                cue.OnGameplayEffectDeactivate();
    }

    private void CueOnTick()
    {
        if (effect.CueDurational == null || effect.CueDurational.Length <= 0) return;
        foreach (var cue in effect.CueDurational) cue.OnTick();
    }
    #endregion
}