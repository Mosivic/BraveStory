using Godot;

namespace Miros.Experiment.StatsAndModifiers;

public partial class PickupNode : Node2D, IVistor
{
    public void Visit<T>(T visitable) where T : Node, IVisitable
    {
        if (visitable is EntityNode entityNode) ApplyPickupEffect(entityNode);
    }

    protected virtual void ApplyPickupEffect(EntityNode entityNode)
    {
    }

    public void OnEntityEntered(EntityNode node)
    {
        node.Accept(this);
        GD.Print("Pickup {name}");
        QueueFree();
    }
}