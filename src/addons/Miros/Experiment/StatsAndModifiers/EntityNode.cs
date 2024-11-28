using Godot;

namespace Miros.Experiment.StatsAndModifiers;

public partial class EntityNode: Node2D,IVisitable
{
    public Stats Stats { get; private set; }
    
    public void Accept(IVistor visitor)=>visitor.Visit(this);

}