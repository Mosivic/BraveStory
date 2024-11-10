using System;

namespace Miros.Core;

public abstract class Ability : AbsState
{
    //描述性质的标签，用来描述Ability的特性表现，比如伤害、治疗、控制等。
    public TagSet Tags { get; protected set; }
    //Ability激活时，Ability持有者当前持有的所有Ability中，拥有【任意】这些标签的Ability会被取消。
    public TagSet CancelAbilityTags { get; protected set; }
    //Ability激活时，Ability持有者当前持有的所有Ability中，拥有【任意】这些标签的Ability会被阻塞激活。
    public TagSet BlockAbilityTags { get; protected set; }
    //Ability激活时，持有者会获得这些标签，Ability被失活时，这些标签也会被移除。
    public TagSet ActivationOwnedTags { get; protected set; }
    //Ability只有在其拥有者拥有【所有】这些标签时才可激活。
    public TagSet ActivationRequiredTags { get; protected set; }
    //Ability在其拥有者拥有【任意】这些标签时不能被激活。
    public TagSet ActivationBlockedTags { get; protected set; }

    public Effect Cooldown { get; protected set; }
    public float CooldownTime { get; protected set; }
    public Effect Cost { get; protected set; }


    protected object[] _abilityArguments = Array.Empty<object>();

    /// <summary>
    /// 获取激活能力时传递给能力的参数。
    /// </summary>
    /// <remarks>
    /// <para>该属性返回一个对象数组，表示激活能力时传入的参数。</para>
    /// <para>即使没有参数传递，该数组也绝不会是 <c>null</c>，在这种情况下，它将是一个空数组。</para>
    /// </remarks>
    public object[] AbilityArguments => _abilityArguments;

    /// <summary>
    /// 获取或设置与能力关联的自定义数据。
    /// </summary>
    /// <remarks>
    /// <para>此属性用于存储能力的自定义信息，以便在能力的不同任务之间共享数据。</para>
    /// <para>例如，可以在一个技能的任务(AbilityTask)中设置此数据，然后在同一个技能的另一个任务(AbilityTask)中检索和使用该数据。</para>
    /// </remarks>
    public object UserData { get; set; }


    public Ability(Persona owner)
    {
        Owner = owner;
    }

    public virtual void Dispose()
    {
        _onActivateResult = null;
        _onEndAbility = null;
        _onCancelAbility = null;
    }
    

    public int Level { get; protected set; }

    public bool IsActive { get; private set; }

    public int ActiveCount { get; private set; }
    protected event Action<AbilityActivateResult> _onActivateResult;
    protected event Action _onEndAbility;
    protected event Action _onCancelAbility;

    public void RegisterActivateResult(Action<AbilityActivateResult> onActivateResult)
    {
        _onActivateResult += onActivateResult;
    }

    public void UnregisterActivateResult(Action<AbilityActivateResult> onActivateResult)
    {
        _onActivateResult -= onActivateResult;
    }

    public void RegisterEndAbility(Action onEndAbility)
    {
        _onEndAbility += onEndAbility;
    }

    public void UnregisterEndAbility(Action onEndAbility)
    {
        _onEndAbility -= onEndAbility;
    }

    public void RegisterCancelAbility(Action onCancelAbility)
    {
        _onCancelAbility += onCancelAbility;
    }

    public void UnregisterCancelAbility(Action onCancelAbility)
    {
        _onCancelAbility -= onCancelAbility;
    }

    public virtual void SetLevel(int level)
    {
        Level = level;
    }

    public virtual AbilityActivateResult CanActivate()
    {
        if (IsActive) return AbilityActivateResult.FailHasActivated;
        if (!CheckGameplayTagsValidTpActivate()) return AbilityActivateResult.FailTagRequirement;
        if (!CheckCost()) return AbilityActivateResult.FailCost;
        if (CheckCooldown().TimeRemaining > 0) return AbilityActivateResult.FailCooldown;

        return AbilityActivateResult.Success;
    }

    /// <summary>
    /// 检查能力激活所需的标签是否满足条件。
    /// </summary>
    /// <returns>如果满足条件，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    private bool CheckGameplayTagsValidTpActivate()
    {
        var hasAllTags = Owner.HasAllTags(ActivationRequiredTags);
        var notHasAnyTags = !Owner.HasAnyTags(ActivationBlockedTags);
        var notBlockedByOtherAbility = true;

        foreach (var kv in Owner.AbilityContainer.Ability())
        {
            var ability = kv.Value;
            if (ability.IsActive)
                if (Tags.HasAnyTags(ability.BlockAbilityTags))
                {
                    notBlockedByOtherAbility = false;
                    break;
                }
        }
        return hasAllTags && notHasAnyTags && notBlockedByOtherAbility;
    }

    /// <summary>
    /// 检查能力消耗是否满足条件。
    /// </summary>
    /// <returns>如果满足条件，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    protected virtual bool CheckCost()
    {
        if (Cost == null) return true;
        if (Cost.DurationPolicy != DurationPolicy.Instant) return true;

        foreach (var modifier in Cost.Modifiers)
        {
            // 常规来说消耗是减法, 但是加一个负数也应该被视为减法
            if (modifier.Operation != ModifierOperation.Add && modifier.Operation != ModifierOperation.Minus) continue;

            var costValue = modifier.CalculateMagnitude(Cost, modifier.Magnitude);
            var attributeCurrentValue =
                Owner.GetAttributeCurrentValue(modifier.AttributeSetName, modifier.AttributeShortName);

            if (modifier.Operation == ModifierOperation.Add)
                if (attributeCurrentValue + costValue < 0) return false;

            if (modifier.Operation == ModifierOperation.Minus)
                if (attributeCurrentValue - costValue < 0) return false;
        }

        return true;
    }

    protected virtual CooldownTimer CheckCooldown()
    {
        return Cooldown == null
            ? new CooldownTimer { TimeRemaining = 0, Duration = CooldownTime }
            : Owner.CheckCooldownFromTags(Cooldown.GrantedTags);
    }


    /// <summary>
    ///     一些技能包含前摇和后摇阶段，前摇阶段可能被打断导致技能释放失败。
    ///     因此，技能释放的实际时机和逻辑（触发消耗和启动冷却）应该由开发者在 Ability 中决定，
    ///     而不是由系统统一规定。
    /// </summary>
    public void DoCost()
    {
        if (Cost != null) Owner.ApplyEffectToSelf(Cost);

        if (Cooldown != null)
        {
            Owner.ApplyEffectToSelf(Cooldown);
             // Actually, it should be set by the ability's cooldown time.
        }
    }

    public virtual bool TryActivateAbility(params object[] args)
    {
        _abilityArguments = args;
        var result = CanActivate();
        var success = result == AbilityActivateResult.Success;
        if (success)
        {
            IsActive = true;
            ActiveCount++;
            Owner.TagAggregator.ApplyAbilityDynamicTag(this);

            ActivateAbility(_abilityArguments);
        }

        _onActivateResult?.Invoke(result);
        return success;
    }

    public virtual void TryEndAbility()
    {
        if (!IsActive) return;
        IsActive = false;
        Owner.TagAggregator.RestoreAbilityDynamicTags(this);
        EndAbility();
        _onEndAbility?.Invoke();
    }

    public virtual void TryCancelAbility()
    {
        if (!IsActive) return;
        IsActive = false;

        Owner.TagAggregator.RestoreAbilityDynamicTags(this);
        CancelAbility();
        _onCancelAbility?.Invoke();
    }

    public void Tick()
    {
        if (IsActive)
        {
            AbilityTick();
        }
    }

    protected virtual void AbilityTick()
    {
    }

    public abstract void ActivateAbility(params object[] args);

    public abstract void CancelAbility();

    public abstract void EndAbility();
}




