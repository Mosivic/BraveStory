using Godot;
using GPC.States;

public class PlayerState : State, IStateGeneric<CharacterBody2D>
{
    public PlayerState(PlayerParams param)
    {
        Params = param;
    }

    public PlayerParams Params { get; set; }

    public float Gravity { get; } = 980;
    public float RunSpeed { get; } = 200;
    public float JumpVeocity { get; } = -300;
    public float FloorAcceleration { get; } = 200 * 5;
    public float AirAcceleration { get; } = 200 * 50;
    public CharacterBody2D Host { get; set; }
}