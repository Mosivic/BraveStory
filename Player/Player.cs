using System.Collections.Generic;
using BraveStory.Scripts;
using BraveStory.State;
using Godot;
using GPC;
using GPC.Evaluator;
using GPC.Job;
using GPC.Scheduler;
using GPC.States;
using GPC.States.Buff;

namespace BraveStory.Player;

public class PlayerNodes : INodes
{
    public ConditionMachine ConditionMachine { get; set; }
    public AnimationPlayer AnimationPlayer { get; init; }
    public Sprite2D Sprite { get; init; }
}

public class PlayerProperties : AbsProperties
{
    public BindableProperty<float> Gravity { get; } = new(980f);
    public BindableProperty<float> RunSpeed { get; } = new(200f);
    public BindableProperty<float> JumpVelocity { get; } = new(-300f);
    public BindableProperty<float> FloorAcceleration { get; } = new(200f * 5);
    public BindableProperty<float> AirAcceleration { get; } = new(200 * 50);
}

public class BindableProperty<T>(T value)
{
    public T Value { get; set; } = value;
}

class PlayerIdleState : PlayerState
{
    public PlayerIdleState(CharacterBody2D host, PlayerNodes node, PlayerProperties properties) : base(host, node, properties)
    {
        Name = "Idle";
        Layer = LayerMap.Movement;
        Priority = 5;
        Type = typeof(Idle);
        IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(false);
    }
}

public partial class Player : CharacterBody2D, IGpcToken
{
    private PlayerNodes _nodes;
    private PlayerProperties _properties;
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

        _nodes = new PlayerNodes
        {
            ConditionMachine = _scheduler,
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"),
            Sprite = GetNode<Sprite2D>("Sprite")
        };
        _properties = new PlayerProperties();


        _scheduler = new ConditionMachine([
            new PlayerState(this, _nodes, _properties)
            {
                Name = "Idle",
                Layer = LayerMap.Movement,
                Priority = 5,
                Type = typeof(Idle),
                IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(false)
            },
            new PlayerState(this, _nodes, _properties)
            {
                Type = typeof(Move),
                Layer = LayerMap.Movement,
                Name = "Run",
                Priority = 2,
                IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(true)
            },
            new PlayerState(this, _nodes, _properties)
            {
                Name = "Jump",
                Priority = 10,
                Layer = LayerMap.Movement,
                Type = typeof(Jump),
                IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true) &&
                                       isOnFloor.Is(true),
                IsFailedFunc = () => Velocity.Y == 0 && isOnFloor.Is(true)
            },
            new Buff()
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
            }
        ]);


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