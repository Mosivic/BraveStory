using Godot;
using GPC.Job.Config;

public class PlayerState : State
{
    public PlayerState(CharacterBody2D characterBody2D)
    {
        Host = characterBody2D;
        AnimationPlayer = Host.GetNode<AnimationPlayer>("AnimationPlayer");
        Sprite = Host.GetNode<Sprite2D>("Sprite2D");
    }


    public CharacterBody2D Host { get; set; }
    public AnimationPlayer AnimationPlayer { get; set; }
    public Sprite2D Sprite { get; set; }

    public float Gravity { get; } = 980;
    public float RunSpeed { get; } = 200;
    public float JumpVeocity { get; } = -300;
}