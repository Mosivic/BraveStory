using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public abstract class Effect : AbsState
{
    private Dictionary<Tag, float> _valueMapWithTag = new Dictionary<Tag, float>();
    private Dictionary<string, float> _valueMapWithName = new Dictionary<string, float>();
    private List<CueDurational> _cueDurationalSpecs = new List<CueDurational>();


    public event Action<Persona, Effect> OnImmunity;
    public event Action<int, int> OnStackCountChanged;

    public Persona Source { get; private set; }
    public Persona Owner { get; private set; }
    public DurationPolicy DurationPolicy { get; private set; }
    public new float Duration { get; set; }
    public new float Period { get; set; }
    public float ActivationTime { get; set; }
    public float Level { get; set; }
    public bool IsApplied { get; set; }
    public bool IsActive { get; set; }
    public int StackCount { get; private set; } = 1;

    public ExecutionCalculation[] Executions { get; set; }
    public GrantedAbilityFromEffect[] GrantedAbilities { get; set; }
    public EffectPeriodTicker PeriodTicker { get; }
    public Effect PeriodExecution { get; private set; }
    public Modifier[] Modifiers { get; private set; }
    public GrantedAbilityFromEffect[] GrantedAbility { get; private set; }



    public Dictionary<string, float> SnapshotSourceAttributes { get; private set; }
    public Dictionary<string, float> SnapshotTargetAttributes { get; private set; }
    public Cue[] CueOnExecute { get; set; }
    public Cue[] CueOnRemove { get; set; }
    public Cue[] CueOnAdd { get; set; }
    public Cue[] CueOnActivate { get; set; }
    public Cue[] CueOnDeactivate { get; set; }
    public Cue[] CueDurational { get; set; }


    //Stacking
    public EffectStacking Stacking { get; private set; }

    // TODO: Expiration Effects 
    public readonly Effect[] PrematureExpirationEffect;
    public readonly Effect[] RoutineExpirationEffectClasses;


    /// <summary>
    /// 游戏效果(GE)拥有的标签, 它们自身没有任何功能且只用于描述GameplayEffect。
    /// 此标签集合用于RemoveGameplayEffectsWithTags的匹配条件, 因此对于Instant型GE没有意义。
    /// 注意：GrantedTags也会被用于RemoveGameplayEffectsWithTags的匹配。
    /// </summary>
    public TagSet AssetTags;

    /// <summary>
    /// 当游戏效果(GE)处于激活状态时，赋予目标的标签集合。
    /// 存于GameplayEffect中且又用于GameplayEffect所应用ASC的标签.
    /// 当GameplayEffect移除时它们也会从ASC中移除. 该标签只作用于持续(Duration)和无限(Infinite)GameplayEffect, 对于Instant型GE没有意义.
    /// 当GE处于非激活状态时，这些标签将被临时移除，直到GE再次激活。
    /// 这些标签同样用于RemoveGameplayEffectsWithTags的匹配。
    /// </summary>
    public TagSet GrantedTags;

    /// <summary>
    /// 当GameplayEffect成功应用后, 如果位于目标上的该GameplayEffect在其Asset Tags或Granted Tags中有任意一个本标签的话, 其就会自目标上移除.
    /// 匹配判断发生在：
    ///   1. Instant GE被应用时；
    ///   2. 非Instant GE每次被激活时；
    ///   3. Period Execution GE(非Instant GE中的PeriodExecution)的每个周期到期时。
    /// </summary>
    public TagSet RemoveEffectsWithTags;

    /// <summary>
    /// ApplicationRequiredTags和ApplicationImmunityTags是一对条件：
    /// 游戏效果能够应用于目标的前提是：
    ///   1. 目标必须拥有ApplicationRequiredTags中的所有标签；
    ///   2. 目标不能拥有ApplicationImmunityTags中的任意标签。
    /// </summary>
    public TagSet ApplicationRequiredTags;

    /// <summary>
    /// ApplicationRequiredTags和ApplicationImmunityTags是一对条件：
    /// 游戏效果能够应用于目标的前提是：
    ///   1. 目标必须拥有ApplicationRequiredTags中的所有标签；
    ///   2. 目标不能拥有ApplicationImmunityTags中的任意标签。
    /// </summary>
    public TagSet ApplicationImmunityTags;

    /// <summary>
    /// 游戏效果(GE)激活所需的标签集合。
    /// 该标签只作用于持续(Duration)和无限(Infinite)GameplayEffect, 对于Instant型GE没有意义.
    /// 一旦GameplayEffect应用后, 这些标签将决定GameplayEffect是开启还是关闭. GameplayEffect可以是关闭但仍然是应用的.
    /// 如果某个GameplayEffect由于不符合Ongoing Tag Requirements而关闭, 但是之后又满足需求了, 那么该GameplayEffect会重新打开并重新应用它的Modifier.
    /// 使用场景包括：
    ///   1. GE应用时，如果满足条件则激活GE，否则不执行任何操作；
    ///   2. 标签发生变化时，如果满足条件则激活GE，否则使GE失效。
    /// </summary>
    public TagSet OngoingRequiredTags;


    public void Init(Persona source, Persona owner, float level = 1)
    {
        Source = source;
        Owner = owner;
        Level = level;
        if (DurationPolicy != DurationPolicy.Instant)
        {
            // PeriodExecution = PeriodExecution?.CreateSpec(source, owner);
            SetGrantedAbility(GrantedAbilities);
        }
        CaptureAttributesSnapshot();
    }

    public void Tick(double delta)
    {
        PeriodTicker.Tick(delta);
    }


    public bool CanApplyTo(Persona target)
    {
        return target.HasAllTags(ApplicationRequiredTags);
    }

    public bool CanRunning(Persona target)
    {
        return target.HasAllTags(OngoingRequiredTags);
    }

    public bool IsImmune(Persona target)
    {
        return target.HasAnyTags(ApplicationImmunityTags);
    }

    public bool StackEqual(Effect effect)
    {
        if (Stacking.StackingType == StackingType.None) return false;
        if (effect.Stacking.StackingType == StackingType.None) return false;
        if (string.IsNullOrEmpty(Stacking.StackingCodeName)) return false;
        if (string.IsNullOrEmpty(effect.Stacking.StackingCodeName)) return false;

        return Stacking.StackingHashCode == effect.Stacking.StackingHashCode;
    }


    public float DurationRemaining()
    {
        if (DurationPolicy == DurationPolicy.Infinite)
            return -1;

        return Mathf.Max(0, Duration - (Time.GetTicksMsec() - ActivationTime));
    }



    public void SetGrantedAbility(GrantedAbilityFromEffect[] grantedAbility)
    {
        GrantedAbility = grantedAbility;
        for (var i = 0; i < grantedAbility.Length; i++)
        {
            GrantedAbility[i] = grantedAbility[i];
        }
    }

    public void SetStacking(EffectStacking stacking)
    {
        Stacking = stacking;
    }

    public void Apply()
    {
        if (IsApplied) return;
        IsApplied = true;

        if (CanRunning(Owner))
        {
            Activate();
        }
    }

    public void DisApply()
    {
        if (!IsApplied) return;
        IsApplied = false;
        Deactivate();
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        ActivationTime = Time.GetTicksMsec();
        TriggerOnActivation();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        TriggerOnDeactivation();
    }


    void TriggerInstantCues(Cue[] cues)
    {
        foreach (var cue in cues) cue.ApplyFrom(this);
    }

    private void TriggerCueOnExecute()
    {
        if (CueOnExecute == null || CueOnExecute.Length <= 0) return;
        TriggerInstantCues(CueOnExecute);
    }

    private void TriggerCueOnAdd()
    {
        if (CueOnAdd != null && CueOnAdd.Length > 0)
            TriggerInstantCues(CueOnAdd);

        if (CueDurational != null && CueDurational.Length > 0)
        {
            _cueDurationalSpecs.Clear();
            foreach (var cueDurational in CueDurational)
            {
                var cueSpec = (CueDurational)cueDurational.ApplyFrom(this);
                if (cueSpec != null) _cueDurationalSpecs.Add(cueSpec);
            }

            foreach (var cue in _cueDurationalSpecs) cue.OnAdd();
        }
    }

    private void TriggerCueOnRemove()
    {
        if (CueOnRemove != null && CueOnRemove.Length > 0)
            TriggerInstantCues(CueOnRemove);

        if (CueDurational != null && CueDurational.Length > 0)
        {
            foreach (var cue in _cueDurationalSpecs) cue.OnRemove();

            _cueDurationalSpecs = null;
        }
    }

    private void TriggerCueOnActivation()
    {
        if (CueOnActivate != null && CueOnActivate.Length > 0)
            TriggerInstantCues(CueOnActivate);

        if (CueDurational != null && CueDurational.Length > 0)
            foreach (var cue in _cueDurationalSpecs)
                cue.OnGameplayEffectActivate();
    }

    private void TriggerCueOnDeactivation()
    {
        if (CueOnDeactivate != null && CueOnDeactivate.Length > 0)
            TriggerInstantCues(CueOnDeactivate);

        if (CueDurational != null && CueDurational.Length > 0)
            foreach (var cue in _cueDurationalSpecs)
                cue.OnGameplayEffectDeactivate();
    }

    private void CueOnTick()
    {
        if (CueDurational == null || CueDurational.Length <= 0) return;
        foreach (var cue in _cueDurationalSpecs) cue.OnTick();
    }

    public void TriggerOnExecute()
    {
        Owner.EffectContainer.RemoveEffectWithAnyTags(RemoveEffectsWithTags);
        Owner.ApplyModFromInstantEffect(this);

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
        Owner.TagAggregator.ApplyEffectDynamicTag(this);
        Owner.EffectContainer.RemoveEffectWithAnyTags(RemoveEffectsWithTags);

        TryActivateGrantedAbilities();
    }

    private void TriggerOnDeactivation()
    {
        TriggerCueOnDeactivation();
        Owner.TagAggregator.RestoreEffectDynamicTags(this);

        TryDeactivateGrantedAbilities();
    }

    public void TriggerOnTick()
    {
        if (DurationPolicy == DurationPolicy.Duration ||
            DurationPolicy == DurationPolicy.Infinite)
            CueOnTick();
    }

    public void TriggerOnImmunity()
    {
        // TODO 免疫触发事件逻辑需要调整
        // onImmunity?.Invoke(Owner, this);
        // onImmunity = null;
    }

    public void RemoveSelf()
    {
        Owner.EffectContainer.RemoveEffect(this);
    }

    private void CaptureAttributesSnapshot()
    {
        SnapshotSourceAttributes = Source.DataSnapshot();
        SnapshotTargetAttributes = Source == Owner ? SnapshotSourceAttributes : Owner.DataSnapshot();
    }

    public void RegisterValue(Tag tag, float value)
    {
        _valueMapWithTag[tag] = value;
    }

    public void RegisterValue(string name, float value)
    {
        _valueMapWithName[name] = value;
    }

    public bool UnregisterValue(Tag tag)
    {
        return _valueMapWithTag.Remove(tag);
    }

    public bool UnregisterValue(string name)
    {
        return _valueMapWithName.Remove(name);
    }

    public float? GetMapValue(Tag tag)
    {
        return _valueMapWithTag.TryGetValue(tag, out var value) ? value : (float?)null;
    }

    public float? GetMapValue(string name)
    {
        return _valueMapWithName.TryGetValue(name, out var value) ? value : (float?)null;
    }

    private void TryActivateGrantedAbilities()
    {
        foreach (var grantedAbility in GrantedAbility)
        {
            if (grantedAbility.ActivationPolicy == GrantedAbilityActivationPolicy.SyncWithEffect)
            {
                Owner.TryActivateAbility(grantedAbility.AbilityName);
            }
        }
    }

    private void TryDeactivateGrantedAbilities()
    {
        foreach (var grantedAbility in GrantedAbility)
        {
            if (grantedAbility.DeactivationPolicy == GrantedAbilityDeactivationPolicy.SyncWithEffect)
            {
                Owner.TryEndAbility(grantedAbility.AbilityName);
            }
        }
    }

    private void TryRemoveGrantedAbilities()
    {
        foreach (var grantedAbility in GrantedAbility)
        {
            if (grantedAbility.RemovePolicy == GrantedAbilityRemovePolicy.SyncWithEffect)
            {
                Owner.TryCancelAbility(grantedAbility.AbilityName);
                Owner.RemoveAbility(grantedAbility.AbilityName);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>Stack Count是否变化</returns>
    public bool RefreshStack()
    {
        var oldStackCount = StackCount;
        RefreshStack(StackCount + 1);
        OnStackCountChange(oldStackCount, StackCount);
        return oldStackCount != StackCount;
    }

    public void RefreshStack(int stackCount)
    {
        if (stackCount <= Stacking.LimitCount)
        {
            // 更新栈数
            StackCount = Mathf.Max(1, stackCount); // 最小层数为1
                                                   // 是否刷新Duration
            if (Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
            {
                RefreshDuration();
            }
            // 是否重置Period
            if (Stacking.PeriodResetPolicy == PeriodResetPolicy.ResetOnSuccessfulApplication)
            {
                PeriodTicker.ResetPeriod();
            }
        }
        else
        {
            // 溢出GE生效
            foreach (var overflowEffect in Stacking.OverflowEffects)
                Owner.ApplyEffectToSelf(overflowEffect);

            if (Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
            {
                if (Stacking.DenyOverflowApplication)
                {
                    //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
                    if (Stacking.ClearStackOnOverflow)
                    {
                        RemoveSelf();
                    }
                }
                else
                {
                    RefreshDuration();
                }
            }
        }
    }

    public void RefreshDuration()
    {
        ActivationTime = Time.GetTicksMsec();
    }

    private void OnStackCountChange(int oldStackCount, int newStackCount)
    {

        OnStackCountChanged?.Invoke(oldStackCount, newStackCount);
    }

    public void RegisterOnStackCountChanged(Action<int, int> callback)
    {
        OnStackCountChanged += callback;
    }

    public void UnregisterOnStackCountChanged(Action<int, int> callback)
    {
        OnStackCountChanged -= callback;
    }
}