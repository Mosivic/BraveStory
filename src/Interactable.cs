using BraveStory;
using Godot;

public partial class Interactable : Area2D
{
    [Signal]
    public delegate void InteractedEventHandler();

    public Interactable()
    {
        CollisionLayer = 0;
        CollisionMask = 0;
        SetCollisionMaskValue(2, true);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public virtual void Interact()
    {
        GD.Print($"[Interact] {Name} is interacted.");
        EmitSignal(SignalName.Interacted);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player) player.Interactions.Add(this);
    }

    private void OnBodyExited(Node body)
    {
        if (body is Player player) player.Interactions.Remove(this);
    }
}