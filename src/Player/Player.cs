using System.Data.Common;
using FSM.Job;
using FSM.Job.Executor;
using FSM.Scheduler;
using FSM.States;
using FSM.States.Buff;
using Godot;
using YamlDotNet.Core.Tokens;

namespace BraveStory.Player;

public partial class Player : CharacterBody2D
{
	private AnimationPlayer _animationPlayer;
	private MultiLayerStateMachineConnect _connect;
	private RayCast2D _footChecker;
	private Node2D _graphic;
	private Sprite2D _sprite;
	private RayCast2D _handChecker;
	private PlayerData Data { get; set; } = new PlayerData();

	private int _jumpCount = 0;
	private int _maxJumpCount = 2;


	public override void _Ready()
	{
		// Compoents
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_graphic = GetNode<Node2D>("Graphic");
		_sprite = _graphic.GetNode<Sprite2D>("Sprite");
		_handChecker = GetNode<RayCast2D>("Graphic/HandChecker");
		_footChecker = GetNode<RayCast2D>("Graphic/FootChecker");

		var ownedTags = new GameplayTagContainer([Tags.Player]);

		// Idle  
		var idle = new HostState<Player>(this)
		{
			Name = "Idle",
			Tag = Tags.Idle,
			Priority = 5,
			JobType = typeof(PlayerJob),
			EnterFunc = s => { PlayAnimation("idle"); _jumpCount = 0; }
		};
		// Jump
		var jump = new HostState<Player>(this)
		{
			Name = "Jump",
			Tag = Tags.Jump,
			Priority = 10,
			JobType = typeof(PlayerJob),
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
			JobType = typeof(PlayerJob),
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
			JobType = typeof(PlayerJob),
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
			JobType = typeof(PlayerJob),
			Priority = 14,
			EnterFunc = s => PlayAnimation("fall"),
			PhysicsUpdateFunc = (state, d) => Fall(d)
		};

		// // Landing
		// var landing = new PlayerState(this, _data)
		// {
		//     Name = "Last",
		//     Layer = movementTag,
		//     JobType = typeof(PlayerJob),
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
			JobType = typeof(PlayerJob),
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
			JobType = typeof(PlayerJob),
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
			JobType = typeof(PlayerJob),
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("attack1");
			},
			ExitCondition = s=> IsAnimationFinished()
		};
		
		var attack11 = new HostState<Player>(this)
		{
			Name = "Attack11",
			Tag = Tags.Attack,
			JobType = typeof(PlayerJob),
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("attack11");
			},
			ExitCondition = s=> IsAnimationFinished()
		};

		var attack111 = new HostState<Player>(this)
		{
			Name = "Attack111",
			Tag = Tags.Attack,
			JobType = typeof(PlayerJob),
			Priority = 16,
			EnterFunc = s =>
			{
				PlayAnimation("attack111");
			},
			ExitCondition = s=> IsAnimationFinished()
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
		// Idle
		transitions.AddTransition(idle, run, KeyDownMove);
		transitions.AddTransition(idle, fall, () => !IsOnFloor());
		transitions.AddTransition(idle, jump, KeyDownJump);
		transitions.AddTransition(idle, attack1, KeyDownAttack);

		// Attack1
		transitions.AddTransition(attack1,idle);
		transitions.AddTransition(attack1,attack11,()=>WaitOverTime(Tags.LayerMovement,0.2f)&&KeyDownAttack(),StateTransitionMode.Delay);

		// Attack11
		transitions.AddTransition(attack11,idle);
		transitions.AddTransition(attack11,attack111,()=>WaitOverTime(Tags.LayerMovement,0.2f)&&KeyDownAttack(),StateTransitionMode.Delay);

		// Attack111
		transitions.AddTransition(attack111,idle);

		// Run
		transitions.AddTransition(run, idle, () => !KeyDownMove());
		transitions.AddTransition(run, jump, KeyDownJump);
		transitions.AddTransition(run, attack1, KeyDownAttack);

		// Jump
		transitions.AddTransition(jump, fall);

		// Fall
		transitions.AddTransition(fall, idle, IsOnFloor);
		transitions.AddTransition(fall, wallSlide, () => _footChecker.IsColliding() && _handChecker.IsColliding() && !KeyDownMove());
		transitions.AddTransition(fall, doubleJump, () => KeyDownJump() && (_jumpCount < _maxJumpCount));

		// DoubleJump
		transitions.AddTransition(doubleJump, fall);

		// WallSlide
		transitions.AddTransition(wallSlide, idle, IsOnFloor);
		transitions.AddTransition(wallSlide, fall, () => !_footChecker.IsColliding());
		transitions.AddTransition(wallSlide, wall_jump, KeyDownJump);

		// WallJump
		transitions.AddTransition(wall_jump, fall);

		// 注册状态和转换
		_connect = new MultiLayerStateMachineConnect([idle, run, jump, doubleJump, fall, wallSlide, wall_jump,attack1,attack11,attack111], ownedTags);
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

	public bool WaitOverTime(GameplayTag layer,double time)
	{
		return _connect.GetCurrentStateTime(layer) > time;
	}
}
