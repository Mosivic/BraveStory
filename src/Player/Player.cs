using System.Collections.Generic;
using System.Linq;
using Godot;
using Miros.Core;

namespace BraveStory;

public partial class Player : Character
{
    private AnimatedSprite2D _animatedSprite;
    private RayCast2D _footChecker;
    private RayCast2D _handChecker;



    private bool _isJumpHolding; // 是否正在按住跳跃键
    private int _jumpCount;
    private float _jumpHoldTime; // 跳跃按住时间计数器
    private int _maxJumpCount = 2;


    public HashSet<Interactable> Interactions { get; set; } = new();


    public override void _Ready()
    {
        base._Ready();
        // Compoents
        _handChecker = GetNode<RayCast2D>("Graphics/HandChecker");
        _footChecker = GetNode<RayCast2D>("Graphics/FootChecker");
        _animatedSprite = GetNode<AnimatedSprite2D>("InteractionIcon");

        Agent = new Agent(this, new StaticTaskProvider(), [typeof(PlayerAttributeSet)]);

        // Idle  
        var idle = new State(Tags.State_Action_Idle, Agent)
            .OnEntered(_ =>
            {
                PlayAnimation("idle");
                _jumpCount = 0;
            });


        // Jump
        var jump = new State(Tags.State_Action_Jump, Agent)
            .OnEntered(_ =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Agent.Attr("JumpVelocity"));
                _jumpCount++;
            });

        // Wall Jump
        var wallJump = new State(Tags.State_Action_WallJump, Agent)
            .OnEntered(_ =>
            {
                PlayAnimation("jump");
                var wallJumpDirectionX = Graphics.Scale.X;
                Velocity = new Vector2(-wallJumpDirectionX * 400, -320);
                _jumpCount = 0;
            });

        // Run
        var run = new State(Tags.State_Action_Run, Agent)
            .OnEntered(_ => PlayAnimation("run"))
            .OnPhysicsUpdated((_, delta) => Move(delta));


        // Fall
        var fall = new State(Tags.State_Action_Fall, Agent)
            .OnEntered(_ => PlayAnimation("fall"))
            .OnPhysicsUpdated((_, delta) => Fall(delta));


        // Double Jump
        var doubleJump = new State(Tags.State_Action_DoubleJump, Agent)
            .OnEntered(_ =>
            {
                PlayAnimation("jump");
                Velocity = new Vector2(Velocity.X, Agent.Attr("JumpVelocity"));
                _jumpCount++;
            });


        // Wall Slide
        var wallSlide = new State(Tags.State_Action_WallSlide, Agent)
            .OnEntered(_ => PlayAnimation("wall_sliding"))
            .OnPhysicsUpdated((_, delta) => WallSlide(delta));


        // Attack1
        var attack1 = new State(Tags.State_Action_Attack1, Agent)
            .OnEntered(_ => PlayAnimation("attack1"))
            .ExitCondition(_ => IsAnimationFinished());


        // Attack11
        var attack11 = new State(Tags.State_Action_Attack11, Agent)
            .OnEntered(_ => PlayAnimation("attack11"))
            .ExitCondition(_ => IsAnimationFinished());

        // Attack111
        var attack111 = new State(Tags.State_Action_Attack111, Agent)
            .OnEntered(_ => PlayAnimation("attack111"))
            .ExitCondition(_ => IsAnimationFinished());

        // Hit
        var hit = new State(Tags.State_Action_Hit, Agent)
            .OnEntered(_ => PlayAnimation("hit"))
            .OnExited(_ => HasHit = false);

        // Die
        var die = new State(Tags.State_Status_Die, Agent)
            .OnEntered(_ =>
            {
                PlayAnimation("die");
                Interactions.Clear();
            })
            .OnPhysicsUpdated((_, _) =>
            {
                if (IsAnimationFinished()) QueueFree();
            });

        // Sliding
        var sliding = new State(Tags.State_Action_Sliding, Agent)
            .OnEntered(_ =>
            {
                PlayAnimation("sliding_start");
            })
            .OnExited(_ => HurtBox.SetDeferred("monitorable", true))
            .OnPhysicsUpdated((_, delta) =>
            {
                if (AnimationPlayer.CurrentAnimation == "sliding_start" && IsAnimationFinished())
                    PlayAnimation("sliding_loop");
                else if (AnimationPlayer.CurrentAnimation == "sliding_loop" && IsAnimationFinished())
                    PlayAnimation("sliding_end");
                Slide(delta);
            });

        // 转换规则
        var transitions = new StateTransitionConfig();
        transitions
            .Add(idle, run, KeyDownMove)
            .Add(idle, fall, () => !IsOnFloor())
            .Add(idle, jump, KeyDownJump)
            .Add(idle, attack1, KeyDownAttack)
            .Add(idle, sliding, KeyDownSliding)
            .Add(attack1, idle)
            .Add(attack1, attack11, () => attack1.RunningTime > 0.2f && KeyDownAttack(), StateTransitionMode.DelayFront)
            .Add(attack11, idle)
            .Add(attack11, attack111, () => attack11.RunningTime > 0.2f && KeyDownAttack(),
                StateTransitionMode.DelayFront)
            .Add(attack111, idle)
            .Add(run, idle, () => !KeyDownMove())
            .Add(run, jump, KeyDownJump)
            .Add(run, attack1, KeyDownAttack)
            .Add(run, sliding, KeyDownSliding)
            .Add(jump, fall)
            .Add(fall, idle, IsOnFloor)
            .Add(fall, wallSlide, () => _footChecker.IsColliding() && _handChecker.IsColliding() && !KeyDownMove())
            .Add(fall, doubleJump, () => KeyDownJump() && _jumpCount < _maxJumpCount)
            .Add(doubleJump, fall)
            .Add(wallSlide, idle, IsOnFloor)
            .Add(wallSlide, fall, () => !_footChecker.IsColliding())
            .Add(wallSlide, wallJump, KeyDownJump)
            .Add(wallJump, fall)
            .Add(hit, idle, IsAnimationFinished)
            .Add(sliding, idle)
            .AddAny(hit, () => HasHit, StateTransitionMode.Force)
            .AddAny(die, () => Agent.Attr("HP") <= 0);

        Agent.CreateMultiLayerStateMachine(Tags.StateLayer_Movement, idle,
            [idle, run, jump, fall, wallJump, doubleJump, wallSlide, attack1, attack11, attack111, hit, die, sliding],
            transitions);

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

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("jump"))
        {
            GD.Print("HP (before): " + Agent.Attr("HP"));
            var effect = new Effect(Tags.Effect_Buff, Agent)
            {
                DurationPolicy = DurationPolicy.Instant,
                Modifiers = [
                    new Modifier(Tags.AttributeSet_Character_Player, Tags.Attribute_HP, 10, ModifierOperation.Add)
                ]
            };
            Agent.AddState(ExecutorType.EffectExecutor, effect);
            GD.Print("HP (after): " + Agent.Attr("HP"));
        }
    }

    // 添加一个统一处理朝向的方法
    private void UpdateFacing(float direction)
    {
        // 如果有输入，根据输入方向转向
        if (!Mathf.IsZeroApprox(direction))
            Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
        // 如果没有输入，根据速度方向转向
        else if (!Mathf.IsZeroApprox(Velocity.X)) Graphics.Scale = new Vector2(Velocity.X < 0 ? -1 : 1, 1);
    }

    private void Move(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, direction * Agent.Attr("RunSpeed"), Agent.Attr("FloorAcceleration"));
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Velocity = velocity;

        UpdateFacing(direction); // 使用新方法处理朝向
        MoveAndSlide();
    }

    private void Fall(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Velocity;

        // 确保空中移动控制合理
        velocity.X = Mathf.MoveToward(
            velocity.X,
            direction * Agent.Attr("RunSpeed"),
            Agent.Attr("AirAcceleration")
        );

        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Velocity = velocity;

        UpdateFacing(direction);
        MoveAndSlide();
    }

    private void WallSlide(double delta)
    {
        var direction = Input.GetAxis("move_left", "move_right");
        var velocity = Velocity;
        velocity.Y = Mathf.Min(velocity.Y + (float)delta * Agent.Attr("Gravity"), 600);
        Velocity = velocity;

        UpdateFacing(direction); // 使用新方法处理朝向
        MoveAndSlide();
    }

    private void WallJumpMove(double delta)
    {
        var velocity = Velocity;
        velocity.Y += (float)delta * Agent.Attr("Gravity");
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
        velocity.X = Mathf.MoveToward(velocity.X, 0, Agent.Attr("SlidingDeceleration") * (float)delta);
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Velocity = velocity;
        MoveAndSlide();
    }
}