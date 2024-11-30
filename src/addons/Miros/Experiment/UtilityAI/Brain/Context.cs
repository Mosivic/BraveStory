using System.Collections.Generic;

namespace Miros.Experiment.UtilityAI;

public class Context
{
    public Brain brain;

    private readonly Dictionary<string, object> _data = [];

    public Context(Brain brain)
    {
        this.brain = brain;
    }

    public T Get<T>(string key) => _data.TryGetValue(key, out var value) ? (T)value : default;

    public void Set<T>(string key, T value) => _data[key] = value;
}

