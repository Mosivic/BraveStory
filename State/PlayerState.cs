using Godot;
using GPC.Scheduler;
using GPC.State;


public struct PlayerParams{
    public ConditionMachine<PlayerState> ConditionMachine{get;set;}
    public CharacterBody2D Host { get; set; }
    public AnimationPlayer AnimationPlayer { get; set; }
    public Panel EvaluatorSpacePanel { get; }
    public Sprite2D Sprite { get; set; }
}

public class PlayerState : State
{
    public PlayerState(PlayerParams param)
    {
        Params = param;
    }

    public PlayerParams Params{get;set;}

    public float Gravity { get; } = 980;
    public float RunSpeed { get; } = 200;
    public float JumpVeocity { get; } = -300;
    public float FloorAcceleration { get; } = 200 * 5;
    public float AirAcceleration { get; } = 200 * 50;
}