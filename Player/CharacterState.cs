using Godot;
using GPC;
using GPC.States;

namespace BraveStory.State;

public abstract class CharacterProperty() 
{
    public abstract AnimationPlayer AnimationPlayer { get; }
    public abstract Sprite2D Sprite { get; }
    public abstract Vector2 Velocity { get; set; }
    
    public BindableProperty<float> Gravity { get; } = new(980f);
    public BindableProperty<float> RunSpeed { get; } = new(200f);
    public BindableProperty<float> JumpVelocity { get; } = new(-300f);
    public BindableProperty<float> FloorAcceleration { get; } = new(200f * 5);
    public BindableProperty<float> AirAcceleration { get; } = new(200 * 50);

    public abstract void MoveAndSlide();
}