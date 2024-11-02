using BraveStory.Job;
using FSM;
using FSM.Evaluator;
using FSM.Job;
using FSM.Job.Executor;
using FSM.Scheduler;
using FSM.States.Buff;
using Godot;
using YamlDotNet.Core.Tokens;

namespace BraveStory.Player;

public partial class Player : CharacterBody2D
{
    private AnimationPlayer _animationPlayer;
    private Connect<StaticJobProvider, ConditionMachine> _connect;
    private RayCast2D _footChecker;
    private Node2D _graphic;
    private RayCast2D _handChecker;
    private PlayerData _data;

    
    public override void _Ready()
    {   
        // Compoents
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _graphic = GetNode<Node2D>("Graphic");
        _handChecker = GetNode<RayCast2D>("Graphic/HandChecker");
        _footChecker = GetNode<RayCast2D>("Graphic/FootChecker");
        
        var evaluatorManager = EvaluatorManager.Instance;
        var entityTags = new GameplayTagContainer();

        // Evaluators
        var isKeyDownJump = evaluatorManager.CreateTaggedEvaluator(
            "is_keydown_jump",() => Input.IsActionJustPressed("jump"),entityTags,Tags.KeyDownJump
        );
        var isKeyDownMove = evaluatorManager.CreateTaggedEvaluator(
            "is_keydown_move",() => Input.IsActionJustPressed("jump"),entityTags,Tags.KeyDownMove
        );
        var isOnFloor = evaluatorManager.CreateTaggedEvaluator(
            "is_on_floor",IsOnFloor,entityTags,Tags.OnFloor
        );
        var isVelocityYPositive = evaluatorManager.CreateTaggedEvaluator(
            "is_velocity_y_positive",() => Velocity.Y >= 0f,entityTags,Tags.VelocityYPositive
        );
        var isFootColliding = evaluatorManager.CreateTaggedEvaluator(
            "is_foot_colliding",() => _footChecker.IsColliding(),entityTags,Tags.FootColliding
        );
        var isHandColliding = evaluatorManager.CreateTaggedEvaluator(
            "is_hand_colliding",() => _handChecker.IsColliding(),entityTags,Tags.HandColliding
        );
        
        
        _data = new PlayerData();

        // Idle  
        var idle = new PlayerState(this, _data)
        {
            Name = "Idle",
            Layer = Tags.LayerMovement,
            Priority = 5,
            JobType = typeof(PlayerJob),
            EnterFunc = s => PlayAnimation("idle")
        };
        // Jump
        var jump = new PlayerState(this, _data)
        {
            Name = "Jump",
            Layer = Tags.LayerMovement,
            Priority = 10,
            JobType = typeof(PlayerJob),
            ConditionTags = [Tags.KeyDownJump,Tags.OnFloor],
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, _data.JumpVelocity);
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Run
        var run = new PlayerState(this, _data)
        {
            JobType = typeof(PlayerJob),
            Layer = Tags.LayerMovement,
            Name = "Run",
            Priority = 2,
            ConditionTags =[Tags.KeyDownMove],
            EnterFunc = s => PlayAnimation("run"),
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // // Fall
        // var fall = new PlayerState(this, _data)
        // {
        //     Name = "Fall",
        //     Layer = movementTag,
        //     JobType = typeof(PlayerJob),
        //     Priority = 14,
        //     IsPreparedFunc = () => isOnFloor.Is(false),
        //     EnterFunc = s => PlayAnimation("jump"),
        //     PhysicsUpdateFunc = (state, d) => Move(d)
        // };

        // // Landing
        // var landing = new PlayerState(this, _data)
        // {
        //     Name = "Last",
        //     Layer = movementTag,
        //     JobType = typeof(PlayerJob),
        //     IsPreparedFunc = () => isOnFloor.LastIs(false) && isOnFloor.Is(true),
        //     Duration = 0.25,
        //     IsFailedFunc = () => false,
        //     Priority = 15,
        //     EnterFunc = s => PlayAnimation("landing")
        // };


        // Double Jump
        var doubleJump = new PlayerState(this, _data)
        {
            Name = "DoubleJump",
            Layer = Tags.LayerMovement,
            JobType = typeof(PlayerJob),
            Priority = 15,
            ConditionTags = [Tags.KeyDownJump,Tags.OnFloor],
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, _data.JumpVelocity);
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Wall Sliding
        var wallSliding = new PlayerState(this, _data)
        {
            Name = "WallSliding",
            Layer = Tags.LayerMovement,
            JobType = typeof(PlayerJob),
            Priority = 16,
            EnterFunc = s =>
            {
                PlayAnimation("wall_sliding");
                _data.Gravity /= 3;
            },
            ExitFunc = s => _data.Gravity *= 3,
            ConditionTags = [Tags.FootColliding,Tags.HandColliding],
            BlockingTags = [Tags.OnFloor],
            PhysicsUpdateFunc = (state, d) => Move(d)
        };


        var addHpBuff = new BuffState
        {
            Name = "AddHp",
            Layer = Tags.LayerBuff,
            JobType = typeof(JobBuff),
            Modifiers =
            [
                new Modifier
                {
                    Property = _data.RunSpeed,
                    Affect = -10,
                    Operator = BuffModifierOperator.Add
                }
            ],
            DurationPolicy = BuffDurationPolicy.Duration,
            Duration = 3,
            Period = 1,
            StackMaxCount = 3,
            ConditionTags = [Tags.KeyDownJump],
            OnPeriodOverFunc = state => GD.Print("PeriodOver"),
            EnterFunc = _ => GD.Print("Enter"),
            ExitFunc = _ => GD.Print("Exit"),
            OnDurationOverFunc = _ => GD.Print("DurationOver"),
            OnApplyModifierFunc = _ => GD.Print($"ApplyModifier RunSpeed : {_data.RunSpeed}"),
            Priority = 0
        };


        _connect = new Connect<StaticJobProvider, ConditionMachine>([
            idle, run, jump, doubleJump, wallSliding
        ]);
    }

    public override void _Process(double delta)
    {
        _connect.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _connect.PhysicsUpdate(delta);
    }

    public void PlayAnimation(string animationName)
    {
        _animationPlayer.Play(animationName);
    }

    public void Move(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * _data.RunSpeed,
            _data.FloorAcceleration);
        velocity.Y += (float)delta * _data.Gravity;
        Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            _graphic.Scale = new Vector2(direction >= 0 ? -1 : 1, 1);

        MoveAndSlide();
    }
}