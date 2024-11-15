using System.Collections.Generic;
using BraveStory;
using Godot;
using Miros.Core;
using static BraveStory.TestData;

public partial class Boar : Character
{
    private readonly EnemyData _data = new();
    private RayCast2D _floorChecker;
    private int _hp = 5;
    private EnemyData Data = new();
    private Vector2 _knockbackVelocity = Vector2.Zero;
    private RayCast2D _playerChecker;
    private RayCast2D _wallChecker;

    public override void _Ready()
    {
        base._Ready();
        // Components
        _wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        _floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        _playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");

        _persona = new Persona(this,new StaticJobProvider());
        // 设置初始朝向为左边
        _graphics.Scale = new Vector2(-1, 1);


        // Idle
        var idle = new State(Tags.Idle)
            .OnEnter(s => PlayAnimation("idle"))
            .To(Tags.Walk, () => !_playerChecker.IsColliding())
            .To(Tags.Run, () => _playerChecker.IsColliding());

        // Walk
        var walk = new State(Tags.Walk)
            .OnEnter(s => PlayAnimation("walk"))
            .OnPhysicsUpdate((s, d) => Patrol(d))
            .To(Tags.Idle, () =>
            (!_floorChecker.IsColliding() && !_playerChecker.IsColliding() && _persona.GetStateBy(Tags.Walk).RunningTime > 2) ||
            (!_floorChecker.IsColliding() && _playerChecker.IsColliding()))
            .To(Tags.Run, () => _playerChecker.IsColliding());

        // Run
        var run = new State(Tags.Run)
            .OnEnter(s => PlayAnimation("run"))
            .OnPhysicsUpdate((s, d) => Chase(d))
            .To(Tags.Idle, () => !_playerChecker.IsColliding());

        // Hit
        var hit = new State(Tags.Hit)
            .OnEnter(s =>
            {
                PlayAnimation("hit");
                // 方式1：根据玩家位置计算击退方向
                var playerPos = (_playerChecker.GetCollider() as Node2D)?.GlobalPosition;
                if (playerPos.HasValue)
                {
                    var direction = (GlobalPosition - playerPos.Value).Normalized();
                    _knockbackVelocity = direction * 300f; // 击退力度
                }
            })
            .OnPhysicsUpdate((s, d) =>
            {
                // 应用击退力
                var velocity = Velocity;
                velocity += _knockbackVelocity;
                velocity.Y += (float)d * _data.Gravity;
                // 逐渐减弱击退效果
                _knockbackVelocity *= 0.8f;
                Velocity = velocity;
                MoveAndSlide();
            })
            .OnExit(s =>
            {
                _hasHit = false;
                _knockbackVelocity = Vector2.Zero;
            })
            .Any(() => _hasHit, StateTransitionMode.Force)
            .To(Tags.Idle, IsAnimationFinished);  

        // Die
        var die = new State(Tags.Die)
            .OnEnter(s => PlayAnimation("die"))
            .OnPhysicsUpdate((s, d) =>
            {
                // 应用击退力
                Velocity = Velocity * 0.9f;
                MoveAndSlide();
            })
            .OnExit(s => QueueFree())
            .Any(() => _hp <= 0);

        _persona.CreateMultiLayerStateMachine(Tags.LayerMovement, idle, [idle, walk, run, hit, die]);
        // State Info Display
        // GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
    }


    private void UpdateFacing(float direction)
    {
        if (!Mathf.IsZeroApprox(direction))
            // 修改朝向逻辑：direction < 0 时朝左（-1），direction > 0 时朝右（1）
            _graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
    }

    private void Patrol(double delta)
    {
        // 检查是否碰到墙壁
        if (_wallChecker.IsColliding())
            // 转向：将 X 缩放在 1 和 -1 之间切换
            _graphics.Scale = new Vector2(_graphics.Scale.X * -1, 1);

        // 移动逻辑
        var velocity = Velocity;
        velocity.X = _data.WalkSpeed * -_graphics.Scale.X; // 注意这里加了负号
        velocity.Y += (float)delta * _data.Gravity;
        Velocity = velocity;
        MoveAndSlide();
    }

    private void Chase(double delta)
    {
        // Implement chase logic when player is detected
        if (_playerChecker.IsColliding())
        {
            var playerPosition = (_playerChecker.GetCollider() as Node2D)?.GlobalPosition;
            if (playerPosition.HasValue)
            {
                var direction = (playerPosition.Value - GlobalPosition).Normalized();
                var velocity = Velocity;
                velocity.X = direction.X * _data.RunSpeed;
                velocity.Y += (float)delta * _data.Gravity;
                Velocity = velocity;
                // 修改朝向：当向左移动时 Scale.X = -1，向右移动时 Scale.X = 1
                _graphics.Scale = new Vector2(direction.X >= 0 ? -1 : 1, 1);
                MoveAndSlide();
            }
        }
    }
}