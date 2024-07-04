using Godot;
using GPC.States;

public class PlayerState(CharacterBody2D host, PlayerParams param) : State
{
    public PlayerParams Params { get; set; } = param;
    public float Gravity { get; } = 980;
    public float RunSpeed { get; } = 200;
    public float JumpVelocity { get; } = -300;
    public float FloorAcceleration { get; } = 200 * 5;
    public float AirAcceleration { get; } = 200 * 50;
    public CharacterBody2D Host { get; set; } = host;
}