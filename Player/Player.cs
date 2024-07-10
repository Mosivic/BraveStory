using BraveStory.Scripts;
using BraveStory.State;
using Godot;
using GPC;
using GPC.Evaluator;
using GPC.Job;
using GPC.Scheduler;
using GPC.States.Buff;

namespace BraveStory.Player;

internal class PlayerState(Player host) : CharacterState
{
    public override AnimationPlayer AnimationPlayer => host.GetNode<AnimationPlayer>("AnimationPlayer");
    public override Sprite2D Sprite => host.GetNode<Sprite2D>("Sprite");
    public override Vector2 Velocity
    {
        get => host.Velocity;
        set => host.Velocity = value;
    }

    public override void MoveAndSlide()
    {
        host.MoveAndSlide();
    }
}

public partial class Player : CharacterBody2D, IGpcToken
{
    private ConditionMachine _scheduler;

    public AbsScheduler GetScheduler()
    {
        return _scheduler;
    }

    public Layer GetRootLayer()
    {
        return LayerMap.Root;
    }

    public override void _Ready()
    {
        Evaluator<bool> isVelocityYPositive = new("isVelocityYPositive", () => Velocity.Y >= 0f);
        Evaluator<bool> isOnFloor = new("isOnFloor", IsOnFloor);


        var idle = new PlayerState(this)
        {
            
            Name = "Idle",
            Layer = LayerMap.Movement,
            Priority = 5,
            Type = typeof(Idle),
            IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(false)
        };
        var jump = new PlayerState(this)
        {
            Name = "Jump",
            Priority = 10,
            Layer = LayerMap.Movement,
            Type = typeof(Jump),
            IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true) &&
                                   isOnFloor.Is(true),
            IsFailedFunc = () => Velocity.Y == 0 && isOnFloor.Is(true)
        };
        var run = new PlayerState(this)
        {
            Type = typeof(Move),
            Layer = LayerMap.Movement,
            Name = "Run",
            Priority = 2,
            IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(true)
        };
        var testBuff = new Buff()
        {
            Name = "AddHp",
            Layer = LayerMap.Buff,
            Modifiers =
            [
                new Modifier
                {
                    Property = _properties.RunSpeed,
                    Operator = BuffModifierOperator.Add,
                    Affect = -10
                }
            ],
            Type = typeof(JobBuff),
            IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true)
        };

        var test = new Buff()
        {
            Name = "Test",
            Type = typeof(JobBuff),
            Layer = LayerMap.Buff,
            Modifiers =
            [
                new Modifier
                {
                    Property = _properties.RunSpeed,
                    Operator = BuffModifierOperator.Add,
                    Affect = -10
                }
            ], 
            Duration = 3,
            Period = 1,
            StackMaxCount = 3,
            DurationPolicy = BuffDurationPolicy.Duration,
            StackCurrentCount = 3,
            PeriodFunc = state =>GD.Print("One Period"),
            EnterFunc = _=> GD.Print("Enter"),
            ExitFunc = _=> GD.Print("Exit"),
            OnStackExpirationFunc = _=> GD.Print("Expirtion")
            
        };
        
        _scheduler = new ConditionMachine([test]);
        
    }

    public override void _Process(double delta)
    {
        _scheduler.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _scheduler.PhysicsUpdate(delta);
    }
}