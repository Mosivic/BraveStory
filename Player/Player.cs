using System;
using System.Collections.Generic;
using BraveStory.Player;
using Godot;
using GPC.Evaluator;
using GPC.Scheduler;
using GPC.States;


public struct PlayerParams
{
    public ConditionMachine ConditionMachine { get; set; }
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
        Evaluator<bool> isVelocityYPositive = new (() => Velocity.Y >= 0f);
        Evaluator<bool> isOnFloor = new(IsOnFloor);
        
        var playerParams = new PlayerParams
        {
            ConditionMachine = _cm,
            Host = this,
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"),
            Sprite = GetNode<Sprite2D>("Sprite")
        };

        var stateSet = new StateSet();
        stateSet.Add(new PlayerState(playerParams)
        {
            Id = "1",
            Type = typeof(Move),
            Name = "Run",
            Priority = 2,
            IsPreparedFunc = () =>isVelocityYPositive.Invoke(true) &&
                                isOnFloor.Invoke(true),
            IsFailedFunc = ()=> Evaluators.IsMoveKeyDown.Invoke(false),

        }).Add(new PlayerState(playerParams)
        {
            Id = "2",
            Name = "Idle",
            Priority = 1,
            Type = typeof(Idle),
            IsPreparedFunc = ()=>Evaluators.IsMoveKeyDown.Invoke(false) &&
                               isOnFloor.Invoke(true),
      
        }).Add(new PlayerState(playerParams)
        {
            Id = "3",
            Name = "Jump",
            Priority = 3,
            Type = typeof(Jump),
            IsPreparedFunc = ()=>Evaluators.IsJumpKeyDown.Invoke(true) &&
                               isOnFloor.Invoke(true),
        }
        ).Add(new PlayerState(playerParams)
        {
            Id = "4",
            Name = "Fall",
            Priority = 9,
            Type = typeof(Fall),
            IsPreparedFunc = ()=> isVelocityYPositive.Invoke(true) &&
                                isOnFloor.Invoke(false),
            IsFailedFunc = ()=> isOnFloor.Invoke(true)
        });


        _cm = new ConditionMachine(stateSet);
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