using System.Collections.Generic;
using BraveStory.State;
using Godot;
using GPC;
using GPC.Scheduler;
using GPC.State;

public class CommonLib : EvaluatorLib
{
	public readonly Evaluator IsJumpKeyDown =
		new(state => Input.IsActionJustPressed("jump"));

	public readonly Evaluator IsMoveKeyDown =
		new(state => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")));
}


public class PlayerLib : EvaluatorLib
{
	public readonly Evaluator IsOnFloor =
		new(state => ((PlayerState)state).Host.IsOnFloor());

	public readonly Evaluator IsVelocityYPositive =
		new(state => (CharacterBody2D)(state.Host).Velocity.Y >= 0f);
}

public struct PlayerParams
{
    public AnimationPlayer AnimationPlayer { get; set; }
    public Panel EvaluatorSpacePanel { get; set; }
    public Sprite2D Sprite { get; set; }
}

public partial class Player : CharacterBody2D
{
	private ConditionMachine<PlayerState> _cm;

	public override void _Ready()
	{
		EvaluatorSpace<CommonLib, PlayerLib> es = new(new CommonLib(), new PlayerLib());

        var p = new PlayerParams(){
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"),
            Sprite = GetNode<Sprite2D>("Sprite2D"),
            EvaluatorSpacePanel = GetNode<Panel>("EvaluatorSpacePanel")

        };

		var ss = new StateSpace();
		ss.Add(new PlayerState(p)
		{
			Id = "1",
			Type = typeof(Move<PlayerState>),
			Name = "Run",
			Priority = 2,
			PreCondition = new Condition(new Dictionary<Evaluator, bool>
			{
				{ es.Global.IsMoveKeyDown, true }
				// { es.Private.IsOnFloor, true }
			}),
			FailedCondition = new Condition(new Dictionary<Evaluator, bool>
			{
				{ es.Global.IsMoveKeyDown, false }
			})
		}).Add(new PlayerState(p)
		{
			Id = "2",
			Name = "Idle",
			Priority = 1,
			Type = typeof(Idle<PlayerState>),
			PreCondition = new Condition(new Dictionary<Evaluator, bool>
			{
				{ es.Global.IsMoveKeyDown, false },
				{ es.Local.IsOnFloor, true }
			})
		}).Add(new PlayerState(p)
			{
				Id = "3",
				Name = "Jump",
				Priority = 3,
				Type = typeof(Jump<PlayerState>),
				PreCondition = new Condition(new Dictionary<Evaluator, bool>
				{
					{ es.Global.IsJumpKeyDown, true },
					{ es.Local.IsOnFloor, true }
				})
			}
		).Add(new PlayerState(p)
		{
			Id = "4",
			Name = "Fall",
			Priority = 9,
			Type = typeof(Fall<PlayerState>),
			PreCondition = new Condition(new Dictionary<Evaluator, bool>
			{
				{ es.Local.IsOnFloor, false },
				{ es.Local.IsVelocityYPositive, true }
			}),
			FailedCondition = new Condition(new Dictionary<Evaluator, bool>
			{
				{ es.Local.IsOnFloor, true }
			})
		});


		_cm = new ConditionMachine<PlayerState>(this,ss);
	}

	public override void _Process(double delta)
	{
		_cm.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_cm.PhysicsUpdate(delta);
	}
}
