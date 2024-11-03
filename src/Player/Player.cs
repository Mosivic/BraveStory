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
    private MultiLayerStateMachineConnect _connect;
    private RayCast2D _footChecker;
    private Node2D _graphic;
    private Sprite2D _sprite;
    private RayCast2D _handChecker;
    public PlayerData Data {get;set;} = new PlayerData();


    private int _jumpCount = 0;
    private int _maxJumpCount = 2;

    public override void _Ready()
    {   
        // Compoents
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _graphic = GetNode<Node2D>("Graphic");
        _sprite = _graphic.GetNode<Sprite2D>("Sprite");
        _handChecker = GetNode<RayCast2D>("Graphic/HandChecker");
        _footChecker = GetNode<RayCast2D>("Graphic/FootChecker");
        
        var evaluatorManager = EvaluatorManager.Instance;
        var ownedTags = new GameplayTagContainer([Tags.Player]);

        // Evaluators
        evaluatorManager.CreateEvaluator(
            "is_keydown_jump",() => Input.IsActionJustPressed("jump"),ownedTags,Tags.KeyDownJump
        );
        evaluatorManager.CreateEvaluator(
            "is_keydown_move",() => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")),ownedTags,Tags.KeyDownMove
        );
        evaluatorManager.CreateEvaluator(
            "is_on_floor",IsOnFloor,ownedTags,Tags.OnFloor
        );
        evaluatorManager.CreateEvaluator(
            "is_on_air",()=>!IsOnFloor(),ownedTags,Tags.OnAir
        );
        evaluatorManager.CreateEvaluator(
            "is_velocity_y_positive",() => Velocity.Y > 0f,ownedTags,Tags.VelocityYPositive
        );
        evaluatorManager.CreateEvaluator(
            "is_foot_colliding",() => _footChecker.IsColliding(),ownedTags,Tags.FootColliding
        );
        evaluatorManager.CreateEvaluator(
            "is_hand_colliding",() => _handChecker.IsColliding(),ownedTags,Tags.HandColliding
        );
        evaluatorManager.CreateEvaluator(
            "over_max_jump_count",() => _jumpCount>=_maxJumpCount,ownedTags,Tags.OverMaxJumpCount
        );
        
        
        // Idle  
        var idle = new HostState<Player>(this)
        {
            Name = "Idle",
            Tag = Tags.Idle,
            Priority = 5,
            JobType = typeof(PlayerJob),
            EnterFunc = s => { PlayAnimation("idle"); _jumpCount=0;}
        };
        // Jump
        var jump = new HostState<Player>(this)
        {
            Name = "Jump",
            Tag = Tags.Jump,
            Priority = 10,
            JobType = typeof(PlayerJob),
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
                _jumpCount+=1;
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Run
        var run = new HostState<Player>(this)
        {
            JobType = typeof(PlayerJob),
            Tag = Tags.Run,
            Name = "Run",
            Priority = 2,
            EnterFunc = s => PlayAnimation("run"),
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Fall
        var fall = new HostState<Player>(this)
        {
            Name = "Fall",
            Tag = Tags.Fall,
            JobType = typeof(PlayerJob),
            Priority = 14,
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
            Tag = Tags.DoubleJump,
            JobType = typeof(PlayerJob),
            Priority = 15,
            EnterFunc = s =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
                _jumpCount+=1;
            },
            PhysicsUpdateFunc = (state, d) => Move(d)
        };

        // Wall Slide
        var wallSlide = new HostState<Player>(this)
        {
            Name = "WallSliding",
            Tag = Tags.WallSlide,
            JobType = typeof(PlayerJob),
            Priority = 16,
            EnterFunc = s =>
            {
                PlayAnimation("wall_sliding");
                Data.Gravity /= 3;
            },
            ExitFunc = s => Data.Gravity *= 3,
            PhysicsUpdateFunc = (state, d) => Move(d)
        };


        var addHpBuff = new BuffState
        {
            Name = "AddHp",
            Tag = Tags.LayerBuff,
            JobType = typeof(JobBuff),
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
            OnPeriodOverFunc = state => GD.Print("PeriodOver"),
            EnterFunc = _ => GD.Print("Enter"),
            ExitFunc = _ => GD.Print("Exit"),
            OnDurationOverFunc = _ => GD.Print("DurationOver"),
            OnApplyModifierFunc = _ => GD.Print($"ApplyModifier RunSpeed : {Data.RunSpeed}"),
            Priority = 0
        };

        // 转换规则
        var transitions = new StateTransitionContainer();
        // Idle
        transitions.AddTransition(new(idle,run,new([Tags.KeyDownMove])));
        transitions.AddTransition(new(idle,fall,new([Tags.OnAir])));
        transitions.AddTransition(new(idle,jump,new([Tags.KeyDownJump])));
        // Run
        transitions.AddTransition(new(run,idle,new(noneRequirements:[Tags.KeyDownMove])));
        transitions.AddTransition(new(run,jump,new([Tags.KeyDownJump])));
        // Jump
        transitions.AddTransition(new(jump,fall,new()));
        // Fall
        transitions.AddTransition(new(fall,idle,new([Tags.OnFloor])));
        transitions.AddTransition(new(fall,doubleJump,new([Tags.KeyDownJump],[],[Tags.OverMaxJumpCount])));
        // DoubleJump
        transitions.AddTransition(new(doubleJump,fall,new()));
        
        
        
        // 注册状态和转换
        _connect = new MultiLayerStateMachineConnect([idle, run, jump, doubleJump,fall, wallSlide],ownedTags);
        _connect.AddLayer(Tags.LayerMovement,idle,transitions);

        // Debug Window
        var canvasLayer = new CanvasLayer();
        AddChild(canvasLayer);
        
        var debugWindow = new TagDebugWindow(ownedTags.GetTags());
        canvasLayer.AddChild(debugWindow);  

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