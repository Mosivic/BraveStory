using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Miros.Experiment.StatsAndModifiers;

public class StatsMediator
{
    private readonly List<StatModifier> _listModifiers = [];
    private readonly Dictionary<StatType,IEnumerable<StatModifier>> _modifiersCache = new();
    private readonly IStatModifierApplicationOrder _order = new NormalStatModifierOrder();
    
    public void PerformQuery(object sender, Query query)
    {
        if (!_modifiersCache.ContainsKey(query.StatType))
        {
            _modifiersCache[query.StatType] = _listModifiers.Where(x => x.Type == query.StatType).ToList();
        }
        query.Value = _order.Apply(_modifiersCache[query.StatType], query.Value);
    }

    public void AddModifier(StatModifier modifier)
    {
        _listModifiers.Add(modifier);
        InvalidateCache(modifier.Type);
        modifier.MarkedForRemoval = false;
        
        modifier.OnDispose += _ => InvalidateCache(modifier.Type);
        modifier.OnDispose += _ => _listModifiers.Remove(modifier);
    }
    
    private void InvalidateCache(StatType statType)
    {
        _modifiersCache.Remove(statType);
    }

    public void Update(double delta)
    {
        foreach (var modifier in _listModifiers)
        {
            modifier.Update(delta);
        }

        foreach (var modifier in _listModifiers.Where(modifier=> modifier.MarkedForRemoval).ToList())
        {
            modifier.Dispose();
        }
    }
}

public class Query(StatType statType, int  value)
{
    public readonly StatType StatType = statType;
    public  int Value = value;
}