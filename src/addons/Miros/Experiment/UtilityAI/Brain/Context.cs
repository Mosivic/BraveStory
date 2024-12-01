using System.Collections.Generic;

namespace Miros.Experiment.UtilityAI;

public class Context
{
    private readonly Dictionary<string, object> _data = [];
    public Brain brain;

    public Context(Brain brain)
    {
        this.brain = brain;
    }

    public T Get<T>(string key)
    {
        return _data.TryGetValue(key, out var value) ? (T)value : default;
    }

    public void Set<T>(string key, T value)
    {
        _data[key] = value;
    }
}