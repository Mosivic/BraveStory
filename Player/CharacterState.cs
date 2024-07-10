using Godot;
using GPC;
using GPC.States;

namespace BraveStory.State;

public abstract class CharacterState : AbsState
{
    public abstract AnimationPlayer AnimationPlayer { get; }
    public abstract Sprite2D Sprite { get; }
    public abstract Vector2 Velocity { get; set; }
    public abstract BindableProperty<float> Gravity { get; set; }
    public abstract BindableProperty<float> RunSpeed { get; set; }
    public abstract BindableProperty<float> JumpVelocity { get; set; }
    public abstract BindableProperty<float> FloorAcceleration { get; set; }
    public abstract BindableProperty<float> AirAcceleration { get; set; }

    public abstract void MoveAndSlide();
}