using System.Collections.Generic;
using System.Linq;
using BraveStory;
using Miros.Core;
using Godot;
using System;



public partial class Player : Character
{
	private RayCast2D _footChecker;
	private RayCast2D _handChecker;
	private AnimatedSprite2D _animatedSprite;

	private PlayerData Data { get; set; } = new PlayerData();
	public HashSet<Interactable> Interactions { get; set; } = new();


	private int _jumpCount = 0;
	private int _maxJumpCount = 2;
	private int _hp = 10;
	private float _slidingSpeed = 0f;
	private const float INITIAL_SLIDING_SPEED = 400f;
	private const float SLIDING_DECELERATION = 600f;
	private const float MIN_SLIDING_SPEED = 20f;


	public override void _Ready()
	{
		base._Ready();
		// Compoents
		_handChecker = GetNode<RayCast2D>("Graphics/HandChecker");
		_footChecker = GetNode<RayCast2D>("Graphics/FootChecker");
		_animatedSprite = GetNode<AnimatedSprite2D>("InteractionIcon");

		// Idle  
		var idle = new State("Idle", Tags.Idle)
		.OnEnter(s => { PlayAnimation("idle"); _jumpCount = 0; });

		// Jump
		var jump = new State("Jump", Tags.Jump)
		.OnEnter(s => {
			PlayAnimation("jump");
			Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
			_jumpCount++;
		});

		// Wall Jump
		var wall_jump = new State("WallJump", Tags.Jump)
		.OnEnter(s => {
			PlayAnimation("jump");
			float wallJumpDirectionX = _graphics.Scale.X;
			Velocity = new Vector2(-wallJumpDirectionX * 400, -320);
			_jumpCount = 0;
		});


		// Run
		var run = new State("Run", Tags.Run)
		.OnEnter(s => PlayAnimation("run"))
		.OnPhysicsUpdate((s, delta) => Move(delta));


		// Fall
		var fall = new State("Fall", Tags.Fall)
		.OnEnter(s => PlayAnimation("fall"))
		.OnPhysicsUpdate((s, delta) => Fall(delta));

		// // Landing
		// var landing = new PlayerState(this, _data)
		// {
		//     Name = "Last",
		//     Layer = movementTag,
		//     JobType = typeof(JobSimple),
		//     IsPreparedFunc = () => isOnFloor.LastIs(false) && isOnFloor.Is(true),
		//     Duration = 0.25,
		//     IsFailedFunc = () => false,
		//     Priority = 15,
		//     EnterFunc = s => PlayAnimation("landing")
		// };


		// Double Jump
		var doubleJump = new State("DoubleJump", Tags.Jump)
		.OnEnter(s => {
			PlayAnimation("jump");
			Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
			_jumpCount++;
		});
				

		// Wall Slide
		var wallSlide = new State("WallSlide", Tags.WallSlide)
		.OnEnter(s => PlayAnimation("wall_sliding"))
		.OnPhysicsUpdate((s, delta) => WallSlide(delta));

		var attack1 = new State("Attack1", Tags.Attack1)
		.OnEnter(s => PlayAnimation("attack1"))
		.OnExitCondition(s => IsAnimationFinished());

		var attack11 = new State("Attack11", Tags.Attack11)
		.OnEnter(s => PlayAnimation("attack11"))
		.OnExitCondition(s => IsAnimationFinished());

		var attack111 = new State("Attack111", Tags.Attack111)
		.OnEnter(s => PlayAnimation("attack111"))
		.OnExitCondition(s => IsAnimationFinished());

		var hit = new State("Hit", Tags.Hit)
		.OnEnter(s => PlayAnimation("hit"))
		.OnExit(s => _hasHit = false);

		var die = new State("Die", Tags.Die)
		.OnEnter(s => {
			PlayAnimation("die");
			Interactions.Clear();
		})
		.OnPhysicsUpdate((s, delta) => {
			if (IsAnimationFinished()) QueueFree();
		});


		// Sliding
		var sliding = new State("Sliding", Tags.Sliding)
		.OnEnter(s => {
			PlayAnimation("sliding_start");
			_slidingSpeed = INITIAL_SLIDING_SPEED * Mathf.Sign(_graphics.Scale.X);
		})
		.OnExit(s => _hurtBox.SetDeferred("monitorable", true))
		.OnPhysicsUpdate((s, delta) => {
			if (_animationPlayer.CurrentAnimation == "sliding_start" && IsAnimationFinished())
			{
				PlayAnimation("sliding_loop");
			}
			else if (_animationPlayer.CurrentAnimation == "sliding_loop" && IsAnimationFinished())
			{
				PlayAnimation("sliding_end");
			}
			Slide(delta);
		});

		// var addHpBuff = new Buff
		// {
		// 	Name = "AddHp",
		// 	Sign = Tags.LayerBuff,
		// 	JobType = typeof(JobBuff),
		// 	Modifiers =
		// 	[
		// 		// new Modifier
		// 		// {
		// 		// 	Property = Data.RunSpeed,
		// 		// 	Affect = -10,
		// 		// 	Operator = ModifierOperation.Add
		// 		// }
		// 	],
		// 	DurationPolicy = DurationPolicy.Duration,
		// 	Duration = 3,
		// 	Period = 1,
		// 	StackMaxCount = 3,
		// 	OnPeriodOverFunc = state => GD.Print("PeriodOver"),
		// 	EnterFunc = _ => GD.Print("Enter"),
		// 	ExitFunc = _ => GD.Print("Exit"),
		// 	OnDurationOverFunc = _ => GD.Print("DurationOver"),
		// 	OnApplyModifierFunc = _ => GD.Print($"ApplyModifier RunSpeed : {Data.RunSpeed}"),
		// 	Priority = 0
		// };


		// 转换规则
		var transitions = new StateTransitionContainer();

		transitions
			.AddTransitionGroup(Tags.Idle, [
				new(Tags.Run, KeyDownMove),
				new(Tags.Fall, () => !IsOnFloor()),
				new(Tags.Jump, KeyDownJump),
				new(Tags.Attack, KeyDownAttack),
				new(Tags.WallSlide, KeyDownSliding)
			])
			.AddTransitionGroup(Tags.Attack1, [
				new(Tags.Idle),
				new(Tags.Attack11, () => attack1.RunningTime > 0.2f && KeyDownAttack(), StateTransitionMode.DelayFront)
			])
			.AddTransitionGroup(Tags.Attack11, [
				new(Tags.Idle),
				new(Tags.Attack111, () => attack11.RunningTime > 0.2f && KeyDownAttack(), StateTransitionMode.DelayFront)
			])
			.AddTransitionGroup(Tags.Attack111, [
				new(Tags.Idle)
			])
			.AddTransitionGroup(Tags.Run, [
				new(Tags.Idle, () => !KeyDownMove()),
				new(Tags.Jump, KeyDownJump),
				new(Tags.Attack, KeyDownAttack),
				new(Tags.WallSlide, KeyDownSliding)
			])
			.AddTransitionGroup(Tags.Jump, [
				new(Tags.Fall)
			])
			.AddTransitionGroup(Tags.Fall, [
				new(Tags.Idle, IsOnFloor),
				new(Tags.WallSlide, () => _footChecker.IsColliding() && _handChecker.IsColliding() && !KeyDownMove()),
				new(Tags.DoubleJump, () => KeyDownJump() && (_jumpCount < _maxJumpCount))
			])
			.AddTransitionGroup(Tags.DoubleJump, [
				new(Tags.Fall)
			])
			.AddTransitionGroup(Tags.WallSlide, [
				new(Tags.Idle, IsOnFloor),
				new(Tags.Fall, () => !_footChecker.IsColliding()),
				new(Tags.WallJump, KeyDownJump)
			])
			.AddTransitionGroup(Tags.WallJump, [
				new(Tags.Fall)
			])
			.AddTransitionGroup(Tags.Hit, [
				new(Tags.Idle, IsAnimationFinished)
			])
			.AddTransitionGroup(Tags.Sliding, [
				new(Tags.Idle)
			]);

		// Special transitions that don't fit the group pattern
		transitions.AddAnyTransition(Tags.Hit, () => _hasHit, StateTransitionMode.Force);
		transitions.AddAnyTransition(Tags.Die, () => _hp <= 0);


		var stateMachine = new MultiLayerStateMachine();
		stateMachine.AddLayer(Tags.LayerMovement, Tags.Idle, transitions);
		_persona.AddScheduler(stateMachine, [idle, run, jump, doubleJump, fall, wallSlide, wall_jump, attack1, attack11, attack111, hit, die, sliding]);

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
			if (Input.IsActionJustPressed("interact"))
			{
				Interactions.Last().Interact();
			}
		}
		else
		{
			_animatedSprite.Visible = false;
		}
	}


	// 添加一个统一处理朝向的方法
	private void UpdateFacing(float direction)
	{
		// 如果有输入，根据输入方向转向
		if (!Mathf.IsZeroApprox(direction))
		{
			_graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
		}
		// 如果没有输入，根据速度方向转向
		else if (!Mathf.IsZeroApprox(Velocity.X))
		{
			_graphics.Scale = new Vector2(Velocity.X < 0 ? -1 : 1, 1);
		}
	}

	public void Move(double delta)
	{
		var direction = Input.GetAxis("move_left", "move_right");
		var velocity = Velocity;
		velocity.X = Mathf.MoveToward(velocity.X, direction * Data.RunSpeed, Data.FloorAcceleration);
		velocity.Y += (float)delta * Data.Gravity;
		Velocity = velocity;

		UpdateFacing(direction);  // 使用新方法处理朝向
		MoveAndSlide();
	}

	public void Fall(double delta)
	{
		var direction = Input.GetAxis("move_left", "move_right");
		var velocity = Velocity;

		// 添加空中移动控制
		velocity.X = Mathf.MoveToward(
			velocity.X,
			direction * Data.RunSpeed,
			Data.AirAcceleration * (float)delta
		);

		velocity.Y += (float)delta * Data.Gravity;
		Velocity = velocity;

		UpdateFacing(direction);
		MoveAndSlide();
	}

	public void WallSlide(double delta)
	{
		var direction = Input.GetAxis("move_left", "move_right");
		var velocity = Velocity;
		velocity.Y = Mathf.Min(velocity.Y + (float)delta * Data.Gravity, 600);
		Velocity = velocity;

		UpdateFacing(direction);  // 使用新方法处理朝向
		MoveAndSlide();
	}

	public void WallJumpMove(double delta)
	{
		var velocity = Velocity;
		velocity.Y += (float)delta * Data.Gravity;
		Velocity = velocity;

		UpdateFacing(0);  // 在空中时只根据速度方向转向
		MoveAndSlide();
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


	public void Slide(double delta)
	{
		var velocity = Velocity;
		_slidingSpeed = Mathf.MoveToward(_slidingSpeed, 0, SLIDING_DECELERATION * (float)delta);
		velocity.X = _slidingSpeed;
		velocity.Y += (float)delta * Data.Gravity;
		Velocity = velocity;
		MoveAndSlide();
	}


	protected override void HandleHit(object sender, HitEventArgs e)
	{
		_hitBox.SetBuffState(new Buff("TestBuff", Tags.LayerBuff)
		{
			JobType = typeof(JobBuff),
		});
	}
}
