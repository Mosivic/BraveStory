using BraveStory.Job;
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
    private AnimationPlayer _animationPlayer;
    private Connect<StaticJobProvider, ConditionMachine> _connect;
    private RayCast2D _footChecker;
    private Node2D _graphic;
    private RayCast2D _handChecker;
    private PlayerProperties _properties;


    
    public override void _Ready()
    {   
        // Compoents
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _graphic = GetNode<Node2D>("Graphic");
        _handChecker = GetNode<RayCast2D>("Graphic/HandChecker");
        _footChecker = GetNode<RayCast2D>("Graphic/FootChecker");
        
        var tagManager = GameplayTagManager.Instance;
        var evaluatorManager = EvaluatorManager.Instance;

        var entityTags = new GameplayTagContainer();

        // 创建要应用的标签
        var tagKeydownJump = tagManager.RequestGameplayTag("Condition.KeyDownJump");
        var tagKeydownMove = tagManager.RequestGameplayTag("Condition.KeyDownMove");
        var tagOnFloor = tagManager.RequestGameplayTag("Condition.OnFloor");
        var tagVelocityYPositive = tagManager.RequestGameplayTag("Condition.VelocityYPositive");

        // Evaluators
        var isKeyDownJump = evaluatorManager.CreateTaggedEvaluator(
            "is_keydown_jump",() => Input.IsActionJustPressed("jump"),entityTags,tagKeydownJump
        );
        var isKeyDownMove = evaluatorManager.CreateTaggedEvaluator(
            "is_keydown_move",() => Input.IsActionJustPressed("jump"),entityTags,tagKeydownJump
        );
        var isOnFloor = evaluatorManager.CreateTaggedEvaluator(
            "is_on_floor",IsOnFloor,entityTags,tagOnFloor
        );
        var isVelocityYPositive = evaluatorManager.CreateTaggedEvaluator(
            "is_velocity_y_positive",() => Velocity.Y >= 0f,entityTags,tagVelocityYPositive
        );


        var movementTag = tagManager.RequestGameplayTag("Movement");
        var buffTag = tagManager.RequestGameplayTag("Buff");
        
        _properties = new PlayerProperties();

        // Idle  
        var idle = new PlayerState(this, _properties)
        {
            Name = "Idle",
            Layer = movementTag,
            Priority = 5,
            JobType = typeof(PlayerJob),
            EnterFunc = s => PlayAnimation("idle")
        };
        // Jump
        var jump = new PlayerState(this, _properties)
        {
            Name = "Jump",
            Layer = movementTag,
            Priority = 10,
            JobType = typeof(PlayerJob),
            ConditionTags = [tagKeydownJump,tagOnFloor],
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, _properties.JumpVelocity.Value);
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Run
        var run = new PlayerState(this, _properties)
        {
            JobType = typeof(PlayerJob),
            Layer = movementTag,
            Name = "Run",
            Priority = 2,
            ConditionTags =[tagKeydownMove],
            EnterFunc = s => PlayAnimation("run"),
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // // Fall
        // var fall = new PlayerState(this, _properties)
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
        // var landing = new PlayerState(this, _properties)
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
        var doubleJump = new PlayerState(this, _properties)
        {
            Name = "DoubleJump",
            Layer = movementTag,
            JobType = typeof(PlayerJob),
            Priority = 15,
            IsPreparedFunc = () => isJumpKeyDown.Is(true) &&
                                    isOnFloor.Is(false),
            IsFailedFunc = () => Velocity.Y == 0 && isOnFloor.Is(true),
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, _properties.JumpVelocity.Value);
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Wall Sliding
        var wallSliding = new PlayerState(this, _properties)
        {
            Name = "WallSliding",
            Layer = movementTag,
            JobType = typeof(PlayerJob),
            Priority = 16,
            EnterFunc = s =>
            {
                PlayAnimation("wall_sliding");
                _properties.Gravity.Value /= 3;
            },
            ExitFunc = s => _properties.Gravity.Value *= 3,
            IsPreparedFunc = () => isOnFloor.Is(false) &&
                                    _handChecker.IsColliding() && _footChecker.IsColliding(),
            PhysicsUpdateFunc = (state, d) => Move(d)
        };


        var addHpBuff = new BuffState
        {
            Name = "AddHp",
            Layer = buffTag,
            JobType = typeof(JobBuff),
            Modifiers =
            [
                new Modifier
                {
                    Property = _properties.RunSpeed,
                    Affect = -10,
                    Operator = BuffModifierOperator.Add
                }
            ],
            DurationPolicy = BuffDurationPolicy.Duration,
            Duration = 3,
            Period = 1,
            StackMaxCount = 3,
            UsePrepareFuncAsRunCondition = false,
            IsPreparedFunc = () => isJumpKeyDown.Is(true),
            OnPeriodOverFunc = state => GD.Print("PeriodOver"),
            EnterFunc = _ => GD.Print("Enter"),
            ExitFunc = _ => GD.Print("Exit"),
            OnDurationOverFunc = _ => GD.Print("DurationOver"),
            OnApplyModifierFunc = _ => GD.Print($"ApplyModifier RunSpeed : {_properties.RunSpeed.Value}"),
            Priority = 0
        };


        _connect = new Connect<StaticJobProvider, ConditionMachine>([
            idle, run, jump, doubleJump, fall, wallSliding
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
        velocity.X = Mathf.MoveToward(velocity.X, direction * _properties.RunSpeed.Value,
            _properties.FloorAcceleration.Value);
        velocity.Y += (float)delta * _properties.Gravity.Value;
        Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            _graphic.Scale = new Vector2(direction >= 0 ? -1 : 1, 1);

        MoveAndSlide();
    }
}