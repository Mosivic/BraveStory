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
			return !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"));
		});

		var IsJumpKeyDown = new PredicateCondition((state)=>{
			return Input.IsActionJustPressed("jump");
		});

		var IsOnFloor = new PredicateCondition((state)=>{
			return this.IsOnFloor();
		});
		
		_cm = new ConditionMachine([
			new PlayerState(this){
				Id = "1",
				Type = typeof(Move),
				Name = "Run",
				Priority = 2,
				Preconditions = new Dictionary<ICondition, bool>(){
					{IsMoveKeyDown,true},{IsOnFloor,true}
				},
				FailedConditions = new Dictionary<ICondition, bool>(){
					{IsOnFloor,false}
				},
			},
			new PlayerState(this){
				Id = "2",
				Name = "Idle",
				Priority = 1,
				Type = typeof(Idle),
				Preconditions = new Dictionary<ICondition, bool>(){
					{IsMoveKeyDown,false},{IsOnFloor,true}
				},
				FailedConditions = new Dictionary<ICondition, bool>(){
					{IsOnFloor,false}
				},
			},
			new PlayerState(this){
				Id = "3",
				Name = "Jump",
				Priority = 3,
				Type = typeof(Jump),
				Preconditions = new Dictionary<ICondition, bool>(){
					{IsJumpKeyDown,true}
				},
				FailedConditions = new Dictionary<ICondition, bool>(){
					{IsOnFloor,true}
				},
				
			}
		]);
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
