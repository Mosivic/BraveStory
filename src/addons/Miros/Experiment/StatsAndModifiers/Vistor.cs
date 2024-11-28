using Godot;

namespace Miros.Experiment.StatsAndModifiers;

public interface IVistor
{
    void Visit<T>(T visitable) where T : Node, IVisitable;
}

public interface IVisitable
{
    void Accept(IVistor visitor);
}