using BraveStory;
using BraveStory.Player;
using FSM.States;
using Godot;
using System;
using YamlDotNet.Core.Tokens;

public partial class Boar : Enemy
{
	private RayCast2D _wallChecker;
	private RayCast2D _floorChecker;
	private RayCast2D _playerChecker;
	private MultiLayerStateMachineConnect _connect;
	private EnemyData _data = new EnemyData();

	private int _hp = 5;
	private bool _hasHit = false;

	public override void _Ready()
	{
		// Components
		_wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
		_floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
		_playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_graphic = GetNode<Node2D>("Graphics");

		// 设置初始朝向为左边
		_graphic.Scale = new Vector2(-1, 1);
		
		var ownedTags = new GameplayTagContainer([Tags.Enemy]);


		// States
		var idle = new HostState<Boar>(this)
		{
			Name = "Idle",
			Tag = Tags.Idle,
			Priority = 5,
			JobType = typeof(EnemyJob),
			EnterFunc = s => PlayAnimation("idle")
		};

		var walk = new HostState<Boar>(this)
		{
			Name = "Walk",
			Tag = Tags.Walk,
			Priority = 10,
			JobType = typeof(EnemyJob),
			EnterFunc = s => PlayAnimation("walk"),
			PhysicsUpdateFunc = (state, d) => Patrol(d)
		};

		var run = new HostState<Boar>(this)
		{
			Name = "Run",
			Tag = Tags.Run,
			Priority = 15,
			JobType = typeof(EnemyJob),
			EnterFunc = s => PlayAnimation("run"),
			PhysicsUpdateFunc = (state, d) => Chase(d)
		};

		var hit = new HostState<Boar>(this)
		{
			Name = "Hit",
			Tag = Tags.Hit,
			Priority = 20,
			JobType = typeof(EnemyJob),
			EnterFunc = s => PlayAnimation("hit"),
			ExitFunc = s => _hasHit = false
		};

		var die = new HostState<Boar>(this)
		{
			Name = "Die",
			Tag = Tags.Hit,
			Priority = 20,
			JobType = typeof(EnemyJob),
			EnterFunc = s => PlayAnimation("die"),
			PhysicsUpdateFunc = (s,d) => {
				if(IsAnimationFinished()) QueueFree();
			}
		};

		// Transitions
		var transitions = new StateTransitionContainer();
		
		// Idle
		transitions.AddTransition(idle, walk,()=>!_playerChecker.IsColliding());
		transitions.AddTransition(idle, run,()=>_playerChecker.IsColliding());
		

		// Walk 
		transitions.AddTransition(walk, idle, () => 
			(!_floorChecker.IsColliding() && !_playerChecker.IsColliding() && WaitOverTime(Tags.LayerMovement, 2)) || 
			(!_floorChecker.IsColliding() && _playerChecker.IsColliding()));
		transitions.AddTransition(walk, run, ()=>_playerChecker.IsColliding());

		// Run 
		transitions.AddTransition(run, idle, ()=>!_playerChecker.IsColliding());

		// Hit 
		transitions.AddAnyTransition(hit, ()=>_hasHit);
		transitions.AddTransition(hit,idle,()=>WaitOverTime(Tags.LayerMovement, 0.4));
		
		// Die
		transitions.AddAnyTransition(die,()=> _hp <= 0);

		// Register states and transitions
		_connect = new MultiLayerStateMachineConnect([idle, walk, run, hit,die], ownedTags);
		_connect.AddLayer(Tags.LayerMovement, idle, transitions);

		// State Info Display
		GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
	}

	private void PlayAnimation(string animationName)
	{
		_animationPlayer.Play(animationName);
	}

	private void UpdateFacing(float direction)
	{
		if (!Mathf.IsZeroApprox(direction))
		{
			// 修改朝向逻辑：direction < 0 时朝左（-1），direction > 0 时朝右（1）
			_graphic.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
		}
	}

	private void Patrol(double delta)
	{
		// 检查是否碰到墙壁
		if (_wallChecker.IsColliding())
		{
			// 转向：将 X 缩放在 1 和 -1 之间切换
			_graphic.Scale = new Vector2(_graphic.Scale.X * -1, 1);
		}

		// 移动逻辑
		var velocity = Velocity;
		velocity.X = _data.WalkSpeed * -_graphic.Scale.X;  // 注意这里加了负号
		velocity.Y += (float)delta * _data.Gravity;
		Velocity = velocity;
		MoveAndSlide();
	}

	private void Chase(double delta)
	{
		// Implement chase logic when player is detected
		if (_playerChecker.IsColliding())
		{
			var playerPosition = (_playerChecker.GetCollider() as Node2D)?.GlobalPosition;
			if (playerPosition.HasValue)
			{
				var direction = (playerPosition.Value - GlobalPosition).Normalized();
				var velocity = Velocity;
				velocity.X = direction.X * _data.RunSpeed;
				velocity.Y += (float)delta * _data.Gravity;
				Velocity = velocity;
				// 修改朝向：当向左移动时 Scale.X = -1，向右移动时 Scale.X = 1
				_graphic.Scale = new Vector2(direction.X >= 0 ? -1 : 1, 1);
				MoveAndSlide();
			}
		}
	}

	public override void _Process(double delta)
	{
		_connect.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_connect.PhysicsUpdate(delta);
	}

	public bool WaitOverTime(GameplayTag layer,double time)
    {
        return _connect.GetCurrentStateTime(layer) > time;
    }

	public bool IsAnimationFinished()
	{
    return !_animationPlayer.IsPlaying() && _animationPlayer.GetQueue().Length == 0;
	}


	public void _on_hurt_box_hurt(Area2D hitbox){
		_hp -= 1;
		_hasHit = true;
	}
}
