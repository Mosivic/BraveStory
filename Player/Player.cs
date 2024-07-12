using System;
using BraveStory.Scripts;
using BraveStory.State;
using FSM;
using FSM.Evaluator;
using FSM.Job;
using FSM.Job.Executor;
using FSM.Scheduler;
using FSM.States.Buff;
using Godot;

namespace BraveStory.Player;


public partial class Player : CharacterBody2D
{
    private Connect<StaticJobProvider, ConditionMachine> _connect;
    public AnimationPlayer AnimationPlayer;
    public Sprite2D Sprite;
    
    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Sprite = GetNode<Sprite2D>("Sprite");
        
        
        Evaluator<bool> isVelocityYPositive = new("isVelocityYPositive", () => Velocity.Y >= 0f);
        Evaluator<bool> isOnFloor = new("isOnFloor", IsOnFloor);

        var properties = new PlayerProperties();
        
        var idle = new PlayerState(this,properties)
        {
            Name = "Idle",
            Layer = LayerMap.Movement,
            Priority = 5,
            Type = typeof(Idle),
            IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(false)
        };
        var jump = new PlayerState(this,properties)
        {
            Name = "Jump",
            Priority = 10,
            Layer = LayerMap.Movement,
            Type = typeof(Jump),
            IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true) &&
                                   isOnFloor.Is(true),
            IsFailedFunc = () => Velocity.Y == 0 && isOnFloor.Is(true)
        };
        var run = new PlayerState(this,properties)
        {
            Type = typeof(Move),
            Layer = LayerMap.Movement,
            Name = "Run",
            Priority = 2,
            IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(true)
        };

        var addHpBuff = new BuffState
        {
            Name = "AddHp",
            Layer = LayerMap.Buff,
            Type = typeof(JobBuff),
            Modifiers =
            [
                new Modifier
                {
                    Property = properties.RunSpeed,
                    Affect = -10,
                    Operator = BuffModifierOperator.Add
                }
            ],
            DurationPolicy = BuffDurationPolicy.Duration,
            Duration = 3,
            Period = 1,
            StackMaxCount = 3,
            UsePrepareFuncAsRunCondition = false,
            IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true),
            OnPeriodOverFunc = state => GD.Print("PeriodOver"),
            EnterFunc = _ => GD.Print("Enter"),
            ExitFunc = _ => GD.Print("Exit"),
            OnDurationOverFunc = _ => GD.Print("DurationOver"),
            OnApplyModifierFunc = _ => GD.Print($"ApplyModifier RunSpeed : {properties.RunSpeed.Value}")
        };

        _connect = new Connect<StaticJobProvider, ConditionMachine>([idle, run, jump]);
    }

    public override void _Process(double delta)
    {
        _connect.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _connect.PhysicsUpdate(delta);
    }
}