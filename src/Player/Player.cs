using System.Collections.Generic;
using System.Linq;
using BraveStory;
using Example;
using Godot;
using Miros.Core;
using static BraveStory.TestData;

namespace BraveStory;
public partial class Player : Character
{
    private const float INITIAL_SLIDING_SPEED = 400f;
    private const float SLIDING_DECELERATION = 600f;
    private const float MIN_SLIDING_SPEED = 20f;
    private AnimatedSprite2D _animatedSprite;
    private RayCast2D _footChecker;
    private RayCast2D _handChecker;
    private int _hp = 10;

    private PlayerData Data;
    private int _jumpCount;
    private int _maxJumpCount = 2;
    private float _slidingSpeed;

    public HashSet<Interactable> Interactions { get; set; } = new();


    public override void _Ready()
    {
        base._Ready();
        // Compoents
        _handChecker = GetNode<RayCast2D>("Graphics/HandChecker");
        _footChecker = GetNode<RayCast2D>("Graphics/FootChecker");
        _animatedSprite = GetNode<AnimatedSprite2D>("InteractionIcon");

        _persona = new Persona(this,new StaticJobProvider());
        _persona.AttributeSetContainer.AddAttributeSet(typeof(PlayerAttributeSet));
        Data = new PlayerData();

        // Idle  
        var idle = new State(Tags.Idle)
            .OnEnter(_ =>
            {
                PlayAnimation("idle");
                _jumpCount = 0;
            })
            .Any(() => _hasHit, StateTransitionMode.Force)
            .To(Tags.Run, KeyDownMove)
            .To(Tags.Fall, () => !IsOnFloor())
            .To(Tags.Jump, KeyDownJump)
            .To(Tags.Attack, KeyDownAttack)
            .To(Tags.WallSlide, KeyDownSliding);

        // Jump
        var jump = new State(Tags.Jump)
            .OnEnter(_ =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
                _jumpCount++;
            })
            .To(Tags.Fall);

        // Wall Jump
        var wall_jump = new State(Tags.Jump)
            .OnEnter(_ =>
            {
                PlayAnimation("jump");
                var wallJumpDirectionX = _graphics.Scale.X;
                Velocity = new Vector2(-wallJumpDirectionX * 400, -320);
                _jumpCount = 0;
            })
            .To(Tags.Fall);

        // Run
        var run = new State(Tags.Run)
            .OnEnter(_ => PlayAnimation("run"))
            .OnPhysicsUpdate((_, delta) => Move(delta))
            .To(Tags.Idle, () => !KeyDownMove())
            .To(Tags.Jump, KeyDownJump)
            .To(Tags.Attack, KeyDownAttack)
            .To(Tags.WallSlide, KeyDownSliding);

        // Fall
        var fall = new State(Tags.Fall)
            .OnEnter(_ => PlayAnimation("fall"))
            .OnPhysicsUpdate((_, delta) => Fall(delta))
            .To(Tags.Idle, IsOnFloor)
            .To(Tags.WallSlide, () => _footChecker.IsColliding() && _handChecker.IsColliding() && !KeyDownMove())
            .To(Tags.DoubleJump, () => KeyDownJump() && _jumpCount < _maxJumpCount);

        // Double Jump
        var doubleJump = new State(Tags.Jump)
            .OnEnter(_ =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Data.JumpVelocity);
                _jumpCount++;
            })
            .To(Tags.Fall);

        // Wall Slide
        var wallSlide = new State(Tags.WallSlide)
            .OnEnter(_ => PlayAnimation("wall_sliding"))
            .OnPhysicsUpdate((_, delta) => WallSlide(delta))
            .To(Tags.Idle, IsOnFloor)
            .To(Tags.Fall, () => !_footChecker.IsColliding())
            .To(Tags.WallJump, KeyDownJump);

        // Attack1
        var attack1 = new State(Tags.Attack1)
            .OnEnter(_ => PlayAnimation("attack1"))
            .OnExitCondition(_ => IsAnimationFinished())
            .To(Tags.Idle)
            .To(Tags.Attack11, () => _persona.GetStateBy(Tags.Attack1).RunningTime > 0.2f && KeyDownAttack(), StateTransitionMode.DelayFront);


        // Attack11
        var attack11 = new State(Tags.Attack11)
            .OnEnter(_ => PlayAnimation("attack11"))
            .OnExitCondition(_ => IsAnimationFinished())
            .To(Tags.Attack111, () => _persona.GetStateBy(Tags.Attack11).RunningTime > 0.2f && KeyDownAttack(), StateTransitionMode.DelayFront);

        // Attack111
        var attack111 = new State(Tags.Attack111)
            .OnEnter(_ => PlayAnimation("attack111"))
            .OnExitCondition(_ => IsAnimationFinished())
            .To(Tags.Idle);

        // Hit
        var hit = new State(Tags.Hit)
            .OnEnter(_ => PlayAnimation("hit"))
            .OnExit(_ => _hasHit = false)
            .To(Tags.Idle, IsAnimationFinished);

        // Die
        var die = new State(Tags.Die)
            .OnEnter(_ =>
            {
                PlayAnimation("die");
                Interactions.Clear();
            })
            .OnPhysicsUpdate((_, _) =>
            {
                if (IsAnimationFinished()) QueueFree();
            })
            .Any(() => _hasHit, StateTransitionMode.Force);

        // Sliding
        var sliding = new State(Tags.Sliding)
            .OnEnter(_ =>
            {
                PlayAnimation("sliding_start");
                _slidingSpeed = INITIAL_SLIDING_SPEED * Mathf.Sign(_graphics.Scale.X);
            })
            .OnExit(_ => _hurtBox.SetDeferred("monitorable", true))
            .OnPhysicsUpdate((_, delta) =>
            {
                if (_animationPlayer.CurrentAnimation == "sliding_start" && IsAnimationFinished())
                    PlayAnimation("sliding_loop");
                else if (_animationPlayer.CurrentAnimation == "sliding_loop" && IsAnimationFinished())
                    PlayAnimation("sliding_end");
                Slide(delta);
            })
            .To(Tags.Idle, IsAnimationFinished);

        _persona.CreateMultiLayerStateMachine(Tags.LayerMovement,idle,
        [idle,run,jump,fall,wall_jump,doubleJump,wallSlide,attack1,attack11,attack111,hit,die,sliding]);

        // Canvas Layer
        var canvasLayer = new CanvasLayer();
        AddChild(canvasLayer);

        // Debug Window
        // var debugWindow = new TagDebugWindow(ownedTags.GetTags());
        // canvasLayer.AddChild(debugWindow);

        // State Info Display
        // GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Interactions.Count != 0)
        {
            _animatedSprite.Visible = true;
            if (Input.IsActionJustPressed("interact")) Interactions.Last().Interact();
        }
        else
        {
            _animatedSprite.Visible = false;
        }
    }


    // 添加一个统一处理朝向的方法
    private void UpdateFacing(float direction)
    {
        // 如果有输入，根据输入方向转向
        if (!Mathf.IsZeroApprox(direction))
            _graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
        // 如果没有输入，根据速度方向转向
        else if (!Mathf.IsZeroApprox(Velocity.X)) _graphics.Scale = new Vector2(Velocity.X < 0 ? -1 : 1, 1);
    }

    private void Move(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * Data.RunSpeed, Data.FloorAcceleration);
        velocity.Y += (float)delta * Data.Gravity;
        Velocity = velocity;

        UpdateFacing(direction); // 使用新方法处理朝向
        MoveAndSlide();
    }

    private void Fall(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Velocity;

        // 添加空中移动控制
        velocity.X = Mathf.MoveToward(
            velocity.X,
            direction * Data.RunSpeed,
            Data.AirAcceleration * (float)delta
        );

        velocity.Y += (float)delta * Data.Gravity;
        Velocity = velocity;

        UpdateFacing(direction);
        MoveAndSlide();
    }

    private void WallSlide(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Velocity;
        velocity.Y = Mathf.Min(velocity.Y + (float)delta * Data.Gravity, 600);
        Velocity = velocity;

        UpdateFacing(direction); // 使用新方法处理朝向
        MoveAndSlide();
    }

    private void WallJumpMove(double delta)
    {
        var velocity = Velocity;
        velocity.Y += (float)delta * Data.Gravity;
        Velocity = velocity;

        UpdateFacing(0); // 在空中时只根据速度方向转向
        MoveAndSlide();
    }

    private bool KeyDownMove()
    {
        return !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"));
    }

    private bool KeyDownJump()
    {
        return Input.IsActionJustPressed("jump");
    }

    private bool KeyDownAttack()
    {
        return Input.IsActionJustPressed("attack");
    }

    private bool KeyDownSliding()
    {
        return Input.IsActionJustPressed("sliding");
    }


    private void Slide(double delta)
    {
        var velocity = Velocity;
        _slidingSpeed = Mathf.MoveToward(_slidingSpeed, 0, SLIDING_DECELERATION * (float)delta);
        velocity.X = _slidingSpeed;
        velocity.Y += (float)delta * Data.Gravity;
        Velocity = velocity;
        MoveAndSlide();
    }


    protected override void HandleHit(object sender, HitEventArgs e)
    {
        _hitBox.SetBuffState(new Buff( Tags.LayerBuff)
        {
            JobType = typeof(JobBuff)
        });
    }
}