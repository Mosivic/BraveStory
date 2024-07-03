using System;
using System.Collections.Generic;
using Godot;
using GPC;
using GPC.Scheduler;
using GPC.States;

public class CommonConditions
{
    public static bool IsJumpKeyDown()
    {
        return Input.IsActionJustPressed("jump");
    }
    
    public static bool IsMoveKeyDown()
    {
        return !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"));
    }
}



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
    

    private bool IsVelocityYPositive()
    {
        return Velocity.Y >= 0f;
    }
    
    public override void _Ready()
    {
        Evaluator<bool> getVelocityValue = new Evaluator<bool>((() => Velocity.Y >= 0f));
        BoolCondition isVelocityYPositive = new BoolCondition(getVelocityValue, true);
        
        var p = new PlayerParams
        {
            ConditionMachine = _cm,
            Host = this,
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"),
            Sprite = GetNode<Sprite2D>("Sprite")
        };

        var ss = new StateSpace();
        ss.Add(new PlayerState(p)
        {
            Id = "1",
            Type = typeof(Move),
            Name = "Run",
            Priority = 2,
            PreCondition =
            [
                !isVelocityYPositive
                //new BoolCondition(IsOnFloor, true)
            ],
            FailedCondition = 
            [
                new BoolCondition(CommonConditions.IsMoveKeyDown,false)
            ]
        }).Add(new PlayerState(p)
        {
            Id = "2",
            Name = "Idle",
            Priority = 1,
            Type = typeof(Idle),
            PreCondition = 
                [
                    new BoolCondition(CommonConditions.IsMoveKeyDown,false),
                    new BoolCondition(IsOnFloor, true)
                ]
        }).Add(new PlayerState(p)
            {
                Id = "3",
                Name = "Jump",
                Priority = 3,
                Type = typeof(Jump),
                PreCondition = 
                [
                    new BoolCondition(CommonConditions.IsJumpKeyDown,true),
                    new BoolCondition(IsOnFloor, true)
                ]
            }
        ).Add(new PlayerState(p)
        {
            Id = "4",
            Name = "Fall",
            Priority = 9,
            Type = typeof(Fall),
            PreCondition = 
            [
                new BoolCondition(IsVelocityYPositive,true),
                new BoolCondition(IsOnFloor, false)
            ],
            FailedCondition =
            [
                new BoolCondition(IsOnFloor, true)
            ],
        });


        _cm = new ConditionMachine(this,ss);
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