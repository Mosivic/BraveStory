using Godot;
using GPC;
using GPC.AI;
using GPC.Job.Config;
using System;
using System.Collections.Generic;

public partial class player : CharacterBody2D
{

	private ConditionMachine _cm;

	public override void _Ready()
	{

		var IsMoveKeyDown = new PredicateCondition((state)=>{
			return Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"));
		});

		var IsJumpKeyDown = new PredicateCondition((state)=>{
			return Input.IsActionJustPressed("jump");
		});

		var IsOnFloor = new PredicateCondition((state)=>{
			return this.IsOnFloor();
		});
		
		_cm = new ConditionMachine([
			new PlayerState(){
				Id = "1",
				Host = this,
				Type = typeof(Move),
				Priority = 2,
				Preconditions = new Dictionary<object, bool>(){
					{IsMoveKeyDown,true}
				},
			},
			new PlayerState(){
			
				Id = "2",
				Host = this,
				Priority = 1,
				Type = typeof(Idle),
				Preconditions = new Dictionary<object, bool>(){
					{IsMoveKeyDown,false}
				},
			},
			new PlayerState(){
				Id = "3",
				Host = this,
				Priority = 3,
				Type = typeof(Jump),
				Preconditions = new Dictionary<object, bool>(){
					{IsJumpKeyDown,true}
				},
				
			}
		]);
	}

	public override void _PhysicsProcess(double delta)
	{
		_cm.PhysicsUpdate(delta);
	}
}
