using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Miros.Core;

namespace BraveStory;

public partial class PlayerShared : CharacterShared
{
	public float KnockbackVelocity { get; set; } = 50.0f;
	public int JumpCount { get; set; } = 0;
	public int MaxJumpCount { get; set; } = 2;
}

public partial class Player : Character<PlayerAgentor, PlayerShared,PlayerAttributeSet>
{
	private AnimatedSprite2D _animatedSprite;
	private RayCast2D _footChecker;
	private RayCast2D _handChecker;

	public HashSet<Interactable> Interactions { get; set; } = new();

	public bool IsFootColliding() => _footChecker.IsColliding();

	public bool IsHandColliding() => _handChecker.IsColliding();


	public void ClearInteractions()
	{
		Interactions.Clear();
	}

	public void SetHitBoxMonitorable(bool monitorable)
	{
		HitBox.SetDeferred("monitorable", monitorable);
	}

	public void SetHurtBoxMonitorable(bool monitorable)
	{
		HurtBox.SetDeferred("monitorable", monitorable);
	}

	public override void _Ready()
	{
		base._Ready();
		// Compoents
		_handChecker = GetNode<RayCast2D>("Graphics/HandChecker");
		_footChecker = GetNode<RayCast2D>("Graphics/FootChecker");
		_animatedSprite = GetNode<AnimatedSprite2D>("InteractionIcon");


		Agentor.BindStators(this,Shared, [
			typeof(IdleAction), typeof(JumpAction), typeof(DoubleJumpAction), 
			typeof(DieAction), typeof(FallAction), typeof(HitAction), 
			typeof(RunAction), typeof(SlidingAction), typeof(WallJumpAction), typeof(WallSlideAction),
			typeof(Attack1Action), typeof(Attack11Action), typeof(Attack111Action)]);

		// Canvas Layer
		var canvasLayer = new CanvasLayer();
		AddChild(canvasLayer);

		
		// Debug Window
		// var debugWindow = new TagDebugWindow(ownedTags.GetTags());
		// canvasLayer.AddChild(debugWindow);

		// State Info Display
		// GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Interactions.Count != 0)
		{
			_animatedSprite.Visible = true;
			if (Input.IsActionJustPressed("interact")) Interactions.Last().Interact();
		}
		else
		{
			_animatedSprite.Visible = false;
		}
	}


	public void UpdateFacing(float direction)
	{
		// 如果有输入，根据输入方向转向
		if (!Mathf.IsZeroApprox(direction))
			Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
		// 如果没有输入，根据速度方向转向
		else if (!Mathf.IsZeroApprox(Velocity.X)) Graphics.Scale = new Vector2(Velocity.X < 0 ? -1 : 1, 1);
	}

	public bool KeyDownMove()
	{
		return !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"));
	}

	public bool KeyDownJump()
	{
		return Input.IsActionJustPressed("jump");
	}

	public bool KeyDownAttack()
	{
		return Input.IsActionJustPressed("attack");
	}

	public bool KeyDownSliding()
	{
		return Input.IsActionJustPressed("sliding");
	}
}
