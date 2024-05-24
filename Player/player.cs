using Godot;
using System;

public partial class player : CharacterBody2D
{
    private float gravity = 980;
    private const float RUN_SPEED = 200;
    private const float JUMP_VELOCITY = -300;

    private Sprite2D _sprite;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        Velocity = new Vector2(direction * RUN_SPEED, Velocity.Y + gravity * (float)delta);

        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);

        if (IsOnFloor())
        {
            if (Mathf.IsZeroApprox(direction))
            {
                _animationPlayer.Play("idle");
            }
            else
            {
                _animationPlayer.Play("run");
            }
        }
        else
        {
            _animationPlayer.Play("jump");
        }

        if (!Mathf.IsZeroApprox(direction))
            _sprite.FlipH = direction < 0;
        
        MoveAndSlide();
    }
}
