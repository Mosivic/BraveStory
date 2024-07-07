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

public class PlayerNodes:INodes
{
    public ConditionMachine ConditionMachine { get; set; }
    public AnimationPlayer AnimationPlayer { get; init; }
    public Sprite2D Sprite { get; init; }
}

public class PlayerProperties() : AbsProperties
{
    public Range<float> Gravity { get; } = new(980f);
    public Range<float> RunSpeed { get; } = new(200f);
    public Range<float> JumpVelocity { get; } = new(-300f);
    public Range<float> FloorAcceleration { get; } = new(200f * 5);
    public Range<float> AirAcceleration { get; } = new(200 * 50);
}

public class Range<T>(T value)
{
    public T Current { get; set; } = value;
    public T Min { get; set; } = value;
    public T Max { get; set; } = value;
}
public partial class Player : CharacterBody2D, IGpcToken
{
    private ConditionMachine _scheduler;
    private PlayerNodes _nodes;
    private PlayerProperties _properties;

    public AbsScheduler GetScheduler() => _scheduler;
    public Layer GetRootLayer() => LayerMap.Root;
    public AbsProperties GetProperties() => _properties;
    
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
        _properties = new();
        
        
        var stateSet = new StateSet();
        stateSet.Add(new PlayerState(this, _nodes, _properties)
        {
            Name = "Idle",
            Layer = LayerMap.Movement,
            Priority = 5,
            Type = typeof(Idle),
            IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(false)
        }).Add(new PlayerState(this, _nodes, _properties)
        {
            Type = typeof(Move),
            Layer = LayerMap.Movement,
            Name = "Run",
            Priority = 2,
            IsPreparedFunc = () => Evaluators.IsMoveKeyDown.Is(true)
        }).Add(new PlayerState(this, _nodes, _properties)
        {
            Name = "Jump",
            Priority = 10,
            Layer = LayerMap.Movement,
            Type = typeof(Jump),
            IsPreparedFunc = () => Evaluators.IsJumpKeyDown.Is(true) &&
                                   isOnFloor.Is(true),
            IsFailedFunc = () => Velocity.Y == 0 && isOnFloor.Is(true)
        }).Add(new Buff()
        {
            Name = "AddHp",
            Layer = LayerMap.Buff,
            Modifiers = [new Modifier()
            {
                Property = _properties.RunSpeed.Current,
                Operator = BuffModifierOperator.Add,
                Affect = 100,
            }],
            Type = typeof(JobBuff),
            IsPreparedFunc = ()=>Evaluators.IsJumpKeyDown.Is(true) 
        });
        

        _scheduler = new ConditionMachine(stateSet);
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