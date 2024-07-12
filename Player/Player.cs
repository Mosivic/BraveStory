using BraveStory.Scripts;
using BraveStory.State;
using Godot;
using FSM;
using FSM.Evaluator;
using FSM.Job;
using FSM.Job.Executor;
using FSM.Scheduler;
using FSM.States.Buff;

namespace BraveStory.Player;

internal class PlayerState(Player host,PlayerState state)  : CharacterState(state)
{
    public override AnimationPlayer AnimationPlayer
    {
        get => host.GetNode<AnimationPlayer>("AnimationPlayer");
        set => throw new System.NotImplementedException();
    }

    public override Sprite2D Sprite
    {
        get => host.GetNode<Sprite2D>("Sprite");
        set => throw new System.NotImplementedException();
    }

    public override Vector2 Velocity
    {
        get => host.Velocity;
        set => host.Velocity = value;
    }

    public override BindableProperty<float> Gravity
    {
        get => host.Gravity;
        set => host.Gravity = value;
    }

    public override BindableProperty<float> RunSpeed
    {
        get => host.RunSpeed;
        set => host.RunSpeed = value;
    }

    public override BindableProperty<float> JumpVelocity
    {
        get => host.JumpVelocity;
        set => host.JumpVelocity = value;
    }

    public override BindableProperty<float> FloorAcceleration
    {
        get => host.FloorAcceleration;
        set => host.FloorAcceleration = value;
    }

    public override BindableProperty<float> AirAcceleration
    {
        get => new(200 * 50);
        set => host.AirAcceleration = value;
    }

    public override void MoveAndSlide()
    {
        host.MoveAndSlide();
    }
}

public partial class Player : CharacterBody2D
{
    private Connect<StaticJobProvider, ConditionMachine> _connect;
    public BindableProperty<float> AirAcceleration = new(200 * 50);
    public BindableProperty<float> FloorAcceleration = new(200f * 5);
    public BindableProperty<float> Gravity = new(980f);
    public BindableProperty<float> JumpVelocity = new(-300f);
    public BindableProperty<float> RunSpeed = new(200f);


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

        var addHpBuff = new BuffState()
        {
            Name = "AddHp",
            Layer = LayerMap.Buff,
            Type = typeof(JobBuff),
            Modifiers = 
                [
                    new Modifier
                    {
                        Property = RunSpeed ,
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
            OnPeriodOverFunc = state => GD.Print($"PeriodOver"),
            EnterFunc = _ => GD.Print($"Enter"),
            ExitFunc = _ => GD.Print("Exit"),
            OnDurationOverFunc = _ => GD.Print("DurationOver"),
            OnApplyModifierFunc = _=> GD.Print($"ApplyModifier RunSpeed : {RunSpeed.Value}")
            
        };
        

        // var test = new BuffState
        // {
        //     Name = "Test",
        //     Type = typeof(JobBuff),
        //     Layer = LayerMap.Buff,
        //     Modifiers =
        //     [
        //         new Modifier
        //         {
        //             Property = _properties.RunSpeed,
        //             Operator = BuffModifierOperator.Add,
        //             Affect = -10
        //         }
        //     ],
        //     Duration = 3,
        //     Period = 1,
        //     StackMaxCount = 3,
        //     DurationPolicy = BuffDurationPolicy.Duration,
        //     StackCurrentCount = 3,
        //     PeriodFunc = state => GD.Print("One Period"),
        //     EnterFunc = _ => GD.Print("Enter"),
        //     ExitFunc = _ => GD.Print("Exit"),
        //     IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true),
        //     OnStackExpirationFunc = _ => GD.Print("Expirtion")
        // };
        _connect = new Connect<StaticJobProvider, ConditionMachine>([idle, run, jump,addHpBuff]);
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