using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Godot;
using GPC;
using GPC.Scheduler;
using GPC.State;

public class CommonLib : EvaluatorLib
{
	public readonly Evaluator<IState,bool> IsJumpKeyDown =
		new(state => Input.IsActionJustPressed("jump"));

	public readonly Evaluator<IState,bool> IsMoveKeyDown =
		new(state => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")));
}


public class PlayerLib : EvaluatorLib
{
	public readonly Func<PlayerState,bool> IsOnFloor =
		new(state => state.Host.IsOnFloor());

	public readonly Evaluator<PlayerState,bool> IsVelocityYPositive =
		new(state => state.Host.Velocity.Y >= 0f);
}

public struct PlayerParams{
    public ConditionMachine ConditionMachine{get;set;}
    public CharacterBody2D Host { get; set; }
    public AnimationPlayer AnimationPlayer { get; set; }
    public Panel EvaluatorSpacePanel { get; }
    public Sprite2D Sprite { get; set; }
}

public partial class Player : CharacterBody2D
{
	private ConditionMachine _cm;

	public override void _Ready()
	{
		EvaluatorSpace<CommonLib, PlayerLib> es = new(new CommonLib(), new PlayerLib());

        var p = new PlayerParams(){
            ConditionMachine = _cm,
            Host = this,
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"),
            Sprite = GetNode<Sprite2D>("Sprite"),

        };

		var ss = new StateSpace();
		ss.Add(new PlayerState(p)
		{
			Id = "1",
			Type = typeof(Move<PlayerState>),
			Name = "Run",
			Priority = 2,
			PreCondition = [
				new Evaluator<PlayerState,bool,CompareFlags.Equal,bool>()
			],new Condition(new Dictionary<Evaluator, bool>
			// {
			// 	{ es.Global.IsMoveKeyDown, true }
			// 	// { es.Private.IsOnFloor, true }
			// }),
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


		_cm = new ConditionMachine<PlayerState>(ss);
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
