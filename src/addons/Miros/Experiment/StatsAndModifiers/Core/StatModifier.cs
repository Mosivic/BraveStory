using System;
using Miros.Utils;

namespace Miros.Experiment.StatsAndModifiers;

public class StatModifier : IDisposable
{
    private readonly CounterTimer _timer;

    public StatModifier(StatType type, IOperationStrategy strategy, float duration)
    {
        Type = type;
        Strategy = strategy;
        if (duration <= 0) return;

        _timer = new CounterTimer(duration);
        _timer.OnTimerStop += () => MarkedForRemoval = true;
        _timer.Start();
    }

    public StatType Type { get; }
    public IOperationStrategy Strategy { get; }

    public bool MarkedForRemoval { get; set; }

    public void Dispose()
    {
        OnDispose?.Invoke(this);
    }

    public void Update(double delta)
    {
        _timer?.Tick(delta);
    }

    public void Handle(object sender, Query query)
    {
        if (query.StatType == Type) query.Value = Strategy.Calculate(query.Value);
    }

    public event Action<object> OnDispose;
}