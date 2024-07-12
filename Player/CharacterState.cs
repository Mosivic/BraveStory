using Godot;
using FSM;
using FSM.States;

namespace BraveStory.State;

public abstract class CharacterState : AbsState
{
    protected CharacterState(CharacterState originState)
    {
        AnimationPlayer = originState.AnimationPlayer;
        Sprite = originState.Sprite;
        Velocity = originState.Velocity;
        Gravity = originState.Gravity;
        RunSpeed = originState.RunSpeed;
        JumpVelocity = originState.JumpVelocity;
        FloorAcceleration = originState.FloorAcceleration;
        AirAcceleration = originState.AirAcceleration;
    }



    public abstract AnimationPlayer AnimationPlayer { get; set; }
    public abstract Sprite2D Sprite { get; set; }
    public abstract Vector2 Velocity { get; set; }
    public abstract BindableProperty<float> Gravity { get; set; }
    public abstract BindableProperty<float> RunSpeed { get; set; }
    public abstract BindableProperty<float> JumpVelocity { get; set; }
    public abstract BindableProperty<float> FloorAcceleration { get; set; }
    public abstract BindableProperty<float> AirAcceleration { get; set; }


    public abstract void MoveAndSlide();
}