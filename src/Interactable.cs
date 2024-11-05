using BraveStory.Player;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Interactable : Area2D
{
    [Signal]
    public delegate void InteractedEventHandler();

    public Interactable()
    {
        CollisionLayer = 0;
        CollisionMask = 0;
        SetCollisionMaskValue(2,true);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public virtual async Task Interact()
    {
        // 基类的异步实现
        await Task.CompletedTask;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            player.Interactions.Add(this);
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is Player player)
        {
            player.Interactions.Remove(this);
        }
    }

}
