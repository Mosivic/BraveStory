using Godot;
using GPC;
using GPC.AI;
using GPC.Job.Config;
using System;
using System.Collections.Generic;
using GPC.Job;

public partial class player : CharacterBody2D
{

	private ConditionMachine<PlayerState> _cm;
	public override void _Ready()
	{
		
		var isMoveKeyDown = new PredicateCondition((state)=>{
			return !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"));
		});

		var isJumpKeyDown = new PredicateCondition((state)=>{
			return Input.IsActionJustPressed("jump");
		});

		var isOnFloor = new PredicateCondition((state)=>{
			return this.IsOnFloor();
		});
		
		_cm = new ConditionMachine<PlayerState>([
			new PlayerState(this){
				Id = "1",
				Type = typeof(Move<PlayerState>),
				Name = "Run",
				Priority = 2,
				Preconditions = new  Dictionary<ICondition, bool>(){
					{isMoveKeyDown,true},{isOnFloor,true}
				},
				FailedConditions = new Dictionary<ICondition, bool>(){
					{isOnFloor,false}
				},
			},
			new PlayerState(this){
				Id = "2",
				Name = "Idle",
				Priority = 1,
				Type = typeof(Idle<PlayerState>),
				Preconditions = new Dictionary<ICondition, bool>(){
					{isMoveKeyDown,false},{isOnFloor,true}
				},
				FailedConditions = new Dictionary<ICondition, bool>(){
					{isOnFloor,false}
				},
			},
			new PlayerState(this){
				Id = "3",
				Name = "Jump",
				Priority = 3,
				Type = typeof(Jump<PlayerState>),
				Preconditions = new Dictionary<ICondition, bool>(){
					{isJumpKeyDown,true},{isOnFloor,true}
				},
				FailedConditions = new Dictionary<ICondition, bool>(){
					
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
