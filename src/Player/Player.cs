using System.Collections.Generic;
using System.Linq.Expressions;
using FSM.Job;
using FSM.States;
using FSM.States.Buff;
using Godot;

namespace BraveStory.Player;

public partial class Player : CharacterBody2D
{
	private AnimationPlayer _animationPlayer;
	private MultiLayerStateMachineConnect _connect;
	private RayCast2D _footChecker;
	private Node2D _graphic;
	private Sprite2D _sprite;
	private RayCast2D _handChecker;
	private Area2D _hurtBox;
	private PlayerData Data { get; set; } = new PlayerData();

	private int _jumpCount = 0;
	private int _maxJumpCount = 2;
	private bool _hasHit = false;
	private int _hp = 10;

	private float _slidingSpeed = 0f;
	private const float INITIAL_SLIDING_SPEED = 400f;
	private const float SLIDING_DECELERATION = 600f;
	private const float MIN_SLIDING_SPEED = 20f;

	public override void _Ready()
	{
		// Compoents
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_graphic = GetNode<Node2D>("Graphic");
		_sprite = _graphic.GetNode<Sprite2D>("Sprite");
		_handChecker = GetNode<RayCast2D>("Graphic/HandChecker");
		_footChecker = GetNode<RayCast2D>("Graphic/FootChecker");
		_hurtBox = GetNode<Area2D>("Graphic/HurtBox");

		var ownedTags = new GameplayTagContainer([Tags.Player]);

		// Idle  
		var idle = new HostState<Player>(this)
		{
			Name = "Idle",
			Tag = Tags.Idle,
			Priority = 5,

			EnterFunc = s => { PlayAnimation("idle"); _jumpCount = 0; }
		};
		// Jump
		var jump = new HostState<Player>(this)
		{
			Name = "Jump",
			Tag = Tags.Jump,
			Priority = 10,

			EnterFunc = s =>
			{
				PlayAnimation("jump");
				Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
				_jumpCount++;

			},
		};

		// Wall Jump
		var wall_jump = new HostState<Player>(this)
		{
			Name = "WallJump",
			Tag = Tags.WallJump,

			Priority = 15,
			EnterFunc = s =>
			{
				PlayAnimation("jump");
				float wallJumpDirectionX = _graphic.Scale.X;
				Velocity = new Vector2(-wallJumpDirectionX * 400, -320);
				_jumpCount = 0;
			},
		};

		// Run
		var run = new HostState<Player>(this)
		{

			Tag = Tags.Run,
			Name = "Run",
			Priority = 2,
			EnterFunc = s => PlayAnimation("run"),
			PhysicsUpdateFunc = (state, d) => Move(d)
		};

		// Fall
		var fall = new HostState<Player>(this)
		{
			Name = "Fall",
			Tag = Tags.Fall,

			Priority = 14,
			EnterFunc = s => PlayAnimation("fall"),
			PhysicsUpdateFunc = (state, d) => Fall(d)
		};

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
		var doubleJump = new HostState<Player>(this)
		{
			Name = "DoubleJump",
			Tag = Tags.DoubleJump,
			Priority = 15,
			EnterFunc = s =>
			{
				PlayAnimation("jump");
				Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
				_jumpCount += 1;
			},
			PhysicsUpdateFunc = (state, d) => Move(d)
		};

		// Wall Slide
		var wallSlide = new HostState<Player>(this)
		{
			Name = "WallSliding",
			Tag = Tags.WallSlide,
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("wall_sliding");
			},
			PhysicsUpdateFunc = (state, d) => WallSlide(d)
		};

		var attack1 = new HostState<Player>(this)
		{
			Name = "Attack1",
			Tag = Tags.Attack,
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("attack1");
			},
			ExitCondition = s => IsAnimationFinished()
		};

		var attack11 = new HostState<Player>(this)
		{
			Name = "Attack11",
			Tag = Tags.Attack,
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("attack11");
			},
			ExitCondition = s => IsAnimationFinished()
		};

		var attack111 = new HostState<Player>(this)
		{
			Name = "Attack111",
			Tag = Tags.Attack,
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("attack111");
			},
			ExitCondition = s => IsAnimationFinished()
		};

		var hit = new HostState<Player>(this)
		{
			Name = "Hit",
			Tag = Tags.Hit,
			Priority = 20,
			EnterFunc = s => PlayAnimation("hit"),
			ExitFunc = s => _hasHit = false
		};

		var die = new HostState<Player>(this)
		{
			Name = "Die",
			Tag = Tags.Die,
			Priority = 20,
			EnterFunc = s => PlayAnimation("die"),
			PhysicsUpdateFunc = (s, d) =>
			{
				if (IsAnimationFinished()) QueueFree();
			}
		};

		// Sliding
		var sliding = new HostState<Player>(this)
		{
			Name = "Sliding",
			Tag = Tags.LayerMovement,
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("sliding_start");
				_slidingSpeed = INITIAL_SLIDING_SPEED * Mathf.Sign(_graphic.Scale.X);
				_hurtBox.SetDeferred("monitorable", false);
			},
			ExitFunc = s => _hurtBox.SetDeferred("monitorable", true),
			PhysicsUpdateFunc = (state, d) =>
			{
				if (_animationPlayer.CurrentAnimation == "sliding_start" && IsAnimationFinished())
				{
					PlayAnimation("sliding_loop");
				}
				else if (_animationPlayer.CurrentAnimation == "sliding_loop" && IsAnimationFinished())
				{
					PlayAnimation("sliding_end");
				}
				Slide(d);
			},
			ExitCondition = s => Mathf.Abs(Velocity.X) < MIN_SLIDING_SPEED 
		};

		var addHpBuff = new BuffState
		{
			Name = "AddHp",
			Tag = Tags.LayerBuff,
			JobType = typeof(JobBuff),
			Modifiers =
			[
				new Modifier
				{
					Property = Data.RunSpeed,
					Affect = -10,
					Operator = BuffModifierOperator.Add
				}
			],
			DurationPolicy = BuffDurationPolicy.Duration,
			Duration = 3,
			Period = 1,
			StackMaxCount = 3,
			OnPeriodOverFunc = state => GD.Print("PeriodOver"),
			EnterFunc = _ => GD.Print("Enter"),
			ExitFunc = _ => GD.Print("Exit"),
			OnDurationOverFunc = _ => GD.Print("DurationOver"),
			OnApplyModifierFunc = _ => GD.Print($"ApplyModifier RunSpeed : {Data.RunSpeed}"),
			Priority = 0
		};

		// 转换规则
		var transitions = new StateTransitionContainer();

		transitions
			.AddTransitionGroup(idle, [
				new(run, KeyDownMove),
				new(fall, () => !IsOnFloor()),
				new(jump, KeyDownJump),
				new(attack1, KeyDownAttack),
				new(sliding, KeyDownSliding)
			])
			.AddTransitionGroup(attack1, [
				new(idle),
				new(attack11, () => WaitOverTime(Tags.LayerMovement, 0.2f) && KeyDownAttack(), StateTransitionMode.Delay)
			])
			.AddTransitionGroup(attack11, [
				new(idle),
				new(attack111, () => WaitOverTime(Tags.LayerMovement, 0.2f) && KeyDownAttack(), StateTransitionMode.Delay)
			])
			.AddTransitionGroup(attack111, [
				new(idle)
			])
			.AddTransitionGroup(run, [
				new(idle, () => !KeyDownMove()),
				new(jump, KeyDownJump),
				new(attack1, KeyDownAttack),
				new(sliding, KeyDownSliding)
			])
			.AddTransitionGroup(jump, [
				new(fall)
			])
			.AddTransitionGroup(fall, [
				new(idle, IsOnFloor),
				new(wallSlide, () => _footChecker.IsColliding() && _handChecker.IsColliding() && !KeyDownMove()),
				new(doubleJump, () => KeyDownJump() && (_jumpCount < _maxJumpCount))
			])
			.AddTransitionGroup(doubleJump, [
				new(fall)
			])
			.AddTransitionGroup(wallSlide, [
				new(idle, IsOnFloor),
				new(fall, () => !_footChecker.IsColliding()),
				new(wall_jump, KeyDownJump)
			])
			.AddTransitionGroup(wall_jump, [
				new(fall)
			])
			.AddTransitionGroup(hit, [
				new(idle, IsAnimationFinished)
			])
			.AddTransitionGroup(sliding, [
				new(idle)
			]);

		// Special transitions that don't fit the group pattern
		transitions.AddAnyTransition(hit, () => _hasHit, StateTransitionMode.Force);
		transitions.AddAnyTransition(die, () => _hp <= 0);

		// 注册状态和转换
		_connect = new MultiLayerStateMachineConnect([
			idle, run, jump, doubleJump, fall, wallSlide,
			wall_jump,attack1,attack11,attack111,hit,die,
			sliding], ownedTags);

		_connect.AddLayer(Tags.LayerMovement, idle, transitions);

		// Canvas Layer
		var canvasLayer = new CanvasLayer();
		AddChild(canvasLayer);

		// Debug Window
		var debugWindow = new TagDebugWindow(ownedTags.GetTags());
		canvasLayer.AddChild(debugWindow);

		// State Info Display
		GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
	}

	public override void _Process(double delta)
	{
		_connect.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_connect.PhysicsUpdate(delta);
	}

	public void PlayAnimation(string animationName)
	{
		_animationPlayer.Play(animationName);
	}

	public bool IsAnimationFinished()
	{
		return !_animationPlayer.IsPlaying() && _animationPlayer.GetQueue().Length == 0;
	}

	// 添加一个统一处理朝向的方法
	private void UpdateFacing(float direction)
	{
		// 如果有输入，根据输入方向转向
		if (!Mathf.IsZeroApprox(direction))
		{
			_graphic.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
		}
		// 如果没有输入，根据速度方向转向
		else if (!Mathf.IsZeroApprox(Velocity.X))
		{
			_graphic.Scale = new Vector2(Velocity.X < 0 ? -1 : 1, 1);
		}
	}

	private void UpdateMovement(double delta, float direction, bool isAirborne = false)
	{
		var velocity = Velocity;
		float acceleration = isAirborne ? Data.AirAcceleration : Data.FloorAcceleration;

		velocity.X = Mathf.MoveToward(
			velocity.X,
			direction * Data.RunSpeed,
			acceleration * (float)delta
		);
		velocity.Y += (float)delta * Data.Gravity;

		Velocity = velocity;
		UpdateFacing(direction);
		MoveAndSlide();
	}

	public void Move(double delta)
	{
		var direction = Input.GetAxis("move_left", "move_right");
		UpdateMovement(delta, direction);
	}

	public void Fall(double delta)
	{
		var direction = Input.GetAxis("move_left", "move_right");
		UpdateMovement(delta, direction, true);
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

	public bool WaitOverTime(GameplayTag layer, double time)
	{
		return _connect.GetCurrentStateTime(layer) > time;
	}

	public void _on_hurt_box_hurt(Area2D hitbox)
	{
		_hp -= 1;
		_hasHit = true;
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
}
