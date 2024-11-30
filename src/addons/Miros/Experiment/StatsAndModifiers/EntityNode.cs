using Godot;

namespace Miros.Experiment.StatsAndModifiers;

public partial class EntityNode: Node2D,IVisitable
{
    public Stats Stats { get; private set; }
    
    public void Accept(IVistor visitor)=>visitor.Visit(this);


    public override void _Process(double delta)
    {
        Stats.Mediator.Update(delta);
    }
}