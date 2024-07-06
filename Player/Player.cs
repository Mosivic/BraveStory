
using BraveStory.Player;
using BraveStory.Scripts;
using Godot;
using GPC;
using GPC.Evaluator;
using GPC.Job;
using GPC.Scheduler;
using GPC.States;

public struct PlayerParams
{
	public ConditionMachine ConditionMachine { get; set; }
	public AnimationPlayer AnimationPlayer { get; init; }
	public Sprite2D Sprite { get; init; }
}

public partial class Player : CharacterBody2D,IGpcToken
{
	private ConditionMachine _scheduler;
	
	public override void _Ready()
	{
		Evaluator<bool> isVelocityYPositive = new("isVelocityYPositive",() => Velocity.Y >= 0f);
		Evaluator<bool> isOnFloor = new("isOnFloor",IsOnFloor);

		var playerParams = new PlayerParams
		{
			ConditionMachine = _scheduler,
			AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"),
			Sprite = GetNode<Sprite2D>("Sprite")
		};

		var stateSet = new StateSet();
		stateSet.Add(new PlayerState(this,playerParams)
		{
			Id = "2",
			Name = "Idle",
			Layer = LayerMap.Behavior,
			Priority = 5,
			Type = typeof(Idle),
			IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(false),
			IsFailedFunc = ()=>Evaluators.IsMoveKeyDown.Is(true)

		}).Add(new PlayerState(this,playerParams)
		{
			Id = "1",
			Type = typeof(Move),
			Layer = LayerMap.Behavior,
			Name = "Run",
			Priority = 2,
			IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(true),
			IsFailedFunc = ()=> Evaluators.IsMoveKeyDown.Is(false)
		});
			/*Add(new PlayerState(playerParams)
		{
			Id = "1",
			Type = typeof(Move),
			Name = "Run",
			Priority = 2,
			IsPreparedFunc = () => isVelocityYPositive.Is(true) &&
								   isOnFloor.Is(true),
			IsFailedFunc = () => Evaluators.IsMoveKeyDown.Is(false)
		}).Add(new PlayerState(playerParams)
		{
			Id = "2",
			Name = "Idle",
			Priority = 1,
			Type = typeof(Idle),
			IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(false) &&
								   isOnFloor.Is(true)
		}).Add(new PlayerState(playerParams)
			{
				Id = "3",
				Name = "Jump",
				Priority = 3,
				Type = typeof(Jump),
				IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true) &&
									   isOnFloor.Is(true)
			}
		).Add(new PlayerState(playerParams)
		{
			Id = "4",
			Name = "Fall",
			Priority = 9,
			Type = typeof(Fall),
			IsPreparedFunc = () => isOnFloor.Is(false),
			IsFailedFunc = () => isOnFloor.Is(true)
		})
		*/
		
		_scheduler = new ConditionMachine(stateSet);
	}

	public override void _Process(double delta)
	{
		_scheduler.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_scheduler.PhysicsUpdate(delta);
	}

	public AbsScheduler GetScheduler()
	{
		return _scheduler;
	}
}
