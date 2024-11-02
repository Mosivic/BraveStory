using System.Data.Common;
using FSM.Job;
using FSM.Job.Executor;
using FSM.Scheduler;
using FSM.States;
using FSM.States.Buff;
using Godot;
using YamlDotNet.Core.Tokens;

namespace BraveStory.Player;

public partial class Player : CharacterBody2D
{
    private AnimationPlayer _animationPlayer;
    private Connect<StaticJobProvider, MultiLayerStateMachine> _connect;
    private RayCast2D _footChecker;
    private Node2D _graphic;
    private RayCast2D _handChecker;
    public PlayerData Data {get;set;} = new PlayerData();


    public override void _Ready()
    {   
        // Compoents
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _graphic = GetNode<Node2D>("Graphic");
        _handChecker = GetNode<RayCast2D>("Graphic/HandChecker");
        _footChecker = GetNode<RayCast2D>("Graphic/FootChecker");
        
        var evaluatorManager = EvaluatorManager.Instance;
        var ownedTags = new GameplayTagContainer([Tags.Player]);

        // Evaluators
        var isKeyDownJump = evaluatorManager.CreateEvaluator(
            "is_keydown_jump",() => Input.IsActionJustPressed("jump"),ownedTags,Tags.KeyDownJump
        );
        var isKeyDownMove = evaluatorManager.CreateEvaluator(
            "is_keydown_move",() => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")),ownedTags,Tags.KeyDownMove
        );
        var isOnFloor = evaluatorManager.CreateEvaluator(
            "is_on_floor",IsOnFloor,ownedTags,Tags.OnFloor
        );
        var isOnAir = evaluatorManager.CreateEvaluator(
            "is_on_air",()=>!IsOnFloor(),ownedTags,Tags.OnAir
        );
        var isVelocityYPositive = evaluatorManager.CreateEvaluator(
            "is_velocity_y_positive",() => Velocity.Y > 0f,ownedTags,Tags.VelocityYPositive
        );
        var isFootColliding = evaluatorManager.CreateEvaluator(
            "is_foot_colliding",() => _footChecker.IsColliding(),ownedTags,Tags.FootColliding
        );
        var isHandColliding = evaluatorManager.CreateEvaluator(
            "is_hand_colliding",() => _handChecker.IsColliding(),ownedTags,Tags.HandColliding
        );
        
        
        // Idle  
        var idle = new HostState<Player>(this)
        {
            Name = "Idle",
            Layer = Tags.LayerMovement,
            OwnedTags =ownedTags,
            Priority = 5,
            JobType = typeof(PlayerJob),
            EnterFunc = s => PlayAnimation("idle")
        };
        // Jump
        var jump = new HostState<Player>(this)
        {
            Name = "Jump",
            Layer = Tags.LayerMovement,
            OwnedTags =ownedTags,
            Priority = 10,
            JobType = typeof(PlayerJob),
            RequiredTags = [Tags.KeyDownJump,Tags.OnFloor],
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Run
        var run = new HostState<Player>(this)
        {
            JobType = typeof(PlayerJob),
            Layer = Tags.LayerMovement,
            OwnedTags =ownedTags,
            Name = "Run",
            Priority = 2,
            RequiredTags =[Tags.KeyDownMove],
            EnterFunc = s => PlayAnimation("run"),
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Fall
        var fall = new HostState<Player>(this)
        {
            Name = "Fall",
            Layer = Tags.LayerMovement,
            OwnedTags =ownedTags,
            JobType = typeof(PlayerJob),
            Priority = 14,
            RequiredTags = [Tags.OnAir],
            EnterFunc = s => PlayAnimation("jump"),
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

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
        var doubleJump = new HostState<Player>(this)
        {
            Name = "DoubleJump",
            Layer = Tags.LayerMovement,
            JobType = typeof(PlayerJob),
            OwnedTags =ownedTags,
            Priority = 15,
            RequiredTags = [Tags.KeyDownJump,Tags.OnFloor],
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Wall Sliding
        var wallSliding = new HostState<Player>(this)
        {
            Name = "WallSliding",
            Layer = Tags.LayerMovement,
            JobType = typeof(PlayerJob),
            OwnedTags =ownedTags,
            Priority = 16,
            EnterFunc = s =>
            {
                PlayAnimation("wall_sliding");
                Data.Gravity /= 3;
            },
            ExitFunc = s => Data.Gravity *= 3,
            RequiredTags = [Tags.FootColliding,Tags.HandColliding],
            BlockedTags = [Tags.OnFloor],
            PhysicsUpdateFunc = (state, d) => Move(d)
        };


        var addHpBuff = new BuffState
        {
            Name = "AddHp",
            Layer = Tags.LayerBuff,
            JobType = typeof(JobBuff),
            OwnedTags =ownedTags,
            Modifiers =
            [
                new Modifier
                {
                    Property = Data.RunSpeed,
                    Affect = -10,
                    Operator = BuffModifierOperator.Add
                }
            ],
            DurationPolicy = BuffDurationPolicy.Duration,
            Duration = 3,
            Period = 1,
            StackMaxCount = 3,
            RequiredTags = [Tags.KeyDownJump],
            OnPeriodOverFunc = state => GD.Print("PeriodOver"),
            EnterFunc = _ => GD.Print("Enter"),
            ExitFunc = _ => GD.Print("Exit"),
            OnDurationOverFunc = _ => GD.Print("DurationOver"),
            OnApplyModifierFunc = _ => GD.Print($"ApplyModifier RunSpeed : {Data.RunSpeed}"),
            Priority = 0
        };


        _connect = new Connect<StaticJobProvider, MultiLayerStateMachine>([
            idle, run, jump, doubleJump,fall, wallSliding
        ]);

        var transitions = new StateTransitionContainer();

        // 添加转换规则
        // Idle -> Run/Jump/Fall
        transitions.AddTransitions(idle, [run,jump,fall]);
        
        // Run -> Idle/Jump
        transitions.AddTransitions(run, [idle,jump]);

        transitions.AddTransitions(fall,[idle,run]);

        _connect.GetScheduler().AddLayer(Tags.LayerMovement,idle,transitions);

        var debugWindow = new TagDebugWindow(ownedTags.GetTags());
        AddChild(debugWindow);

    }

    public override void _Process(double delta)
    {
        _connect.Update(delta);
        EvaluatorManager.Instance.ProcessEvaluators();
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
        velocity.X = Mathf.MoveToward(velocity.X, direction * Data.RunSpeed,
            Data.FloorAcceleration);
        velocity.Y += (float)delta * Data.Gravity;
        Velocity = velocity;

        if (!Mathf.IsZeroApprox(direction))
            _graphic.Scale = new Vector2(direction >= 0 ? -1 : 1, 1);

        MoveAndSlide();
    }
}