using System.Collections.Generic;

namespace Miros.Core;

public class AbilityScheduler
{
    private readonly Persona _owner;
    private readonly Dictionary<string, Ability> _abilities = new();
    private readonly List<Ability> _cachedAbilities = new();

    public Dictionary<string, Ability> Abilities => _abilities;
    public AbilityScheduler(Persona owner)
    {
        _owner = owner;
    }

    public void Tick()
    {
        _cachedAbilities.AddRange(_abilities.Values);

        foreach (var ability in _cachedAbilities)
        {
            ability.Tick();
        }

        _cachedAbilities.Clear();
    }

    public void GrantAbility(Ability ability)
    {
        if (_abilities.ContainsKey(ability.Name)) return;
        _abilities.Add(ability.Name, ability);
    }

    public void RemoveAbility(Ability ability)
    {
        RemoveAbility(ability.Name);
    }

    public void RemoveAbility(string abilityName)
    {
        if (!_abilities.ContainsKey(abilityName)) return;

        EndAbility(abilityName);
        _abilities[abilityName].Dispose();
        _abilities.Remove(abilityName);
    }

    public bool TryActivateAbility(string abilityName, params object[] args)
    {
        if (!_abilities.ContainsKey(abilityName))
            return false;
        

        if (!_abilities[abilityName].TryActivateAbility(args)) return false;

        var tags = _abilities[abilityName].CancelAbilityTags;
        foreach (var kv in _abilities)
        {
            var abilityTag = kv.Value.Tags;
            if (abilityTag.HasAnyTags(tags))
            {
                _abilities[kv.Key].TryCancelAbility();
            }
        }

        return true;
    }

    public void EndAbility(string abilityName)
    {
        if (!_abilities.ContainsKey(abilityName)) return;
        _abilities[abilityName].TryEndAbility();
    }

    public void CancelAbility(string abilityName)
    {
        if (!_abilities.ContainsKey(abilityName)) return;
        _abilities[abilityName].TryCancelAbility();
    }

    void CancelAbilitiesByTag(Tag tags)
    {
        foreach (var kv in _abilities)  
        {
            var abilityTag = kv.Value.Tags;
            if (abilityTag.HasAnyTags(tags))
            {
                _abilities[kv.Key].TryCancelAbility();
            }
        }
    }

    public Dictionary<string, Ability> Ability() => _abilities;

    public void CancelAllAbilities()
    {
        foreach (var kv in _abilities)
            _abilities[kv.Key].TryCancelAbility();
    }

    public bool HasAbility(string abilityName) => _abilities.ContainsKey(abilityName);

    public bool HasAbility(Ability ability) => HasAbility(ability.Name);
}