using BraveStory;
using Godot;
using Miros.Core;

public partial class Boar : Character
{
    private RayCast2D _floorChecker;
    private int _hp = 5;
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

        
        // 设置初始朝向为左边
        Graphics.Scale = new Vector2(-1, 1);

        // Idle
        var idle = new State(Tags.State_Action_Idle, Agent)
            .OnEntered(s => PlayAnimation("idle"));

        // Walk
        var walk = new State(Tags.State_Action_Walk, Agent)
            .OnEntered(s => PlayAnimation("walk"))
            .OnPhysicsUpdated((s, d) => Patrol(d));

        // Run
        var run = new State(Tags.State_Action_Run, Agent)
            .OnEntered(s => PlayAnimation("run"))
            .OnPhysicsUpdated((s, d) => Chase(d));

        // Hit
        var hit = new State(Tags.State_Action_Hit, Agent)
            .OnEntered(s =>
            {
                Hurt = false;
                PlayAnimation("hit");
                // 方式1：根据玩家位置计算击退方向
                var playerPos = (_playerChecker.GetCollider() as Node2D)?.GlobalPosition;
                if (playerPos.HasValue)
                {
                    var direction = (GlobalPosition - playerPos.Value).Normalized();
                    _knockbackVelocity = new Vector2(direction.X * 100f, 0); // 击退力度
                }
            })
            .OnPhysicsUpdated((s, d) =>
            {
                // 应用击退力
                var velocity = Velocity;
                velocity += _knockbackVelocity;
                velocity.Y += (float)d * Agent.Attr("Gravity");
                // 逐渐减弱击退效果
                _knockbackVelocity *= 0.8f;
                Velocity = velocity;
                MoveAndSlide();
            })
            .OnExited(s =>
            {
                Hurt = false;
                _knockbackVelocity = Vector2.Zero;
            });

        // Die
        var die = new State(Tags.State_Status_Die, Agent)
            .OnEntered(s => PlayAnimation("die"))
            .OnPhysicsUpdated((s, d) =>
            {
                // 应用击退力
                Velocity = Velocity * 0.9f;
                MoveAndSlide();
                if(IsAnimationFinished()) QueueFree();
            });
 


        // Transitions
        var transitions = new StateTransitionConfig();
        transitions
            .Add(idle, walk, () => !_playerChecker.IsColliding())
            .Add(idle, run, () => _playerChecker.IsColliding())
            .Add(walk, idle, () =>
                (!_floorChecker.IsColliding() && !_playerChecker.IsColliding() && walk.RunningTime > 2) ||
                (!_floorChecker.IsColliding() && _playerChecker.IsColliding()))
            .Add(walk, run, () => _playerChecker.IsColliding())
            .Add(run, idle, () => !_playerChecker.IsColliding())
            .AddAny(hit, () => Hurt)
            .Add(hit, idle, IsAnimationFinished)
            .AddAny(die, () => Agent.Attr("HP") <= 0);

        Agent.CreateMultiLayerStateMachine(Tags.StateLayer_Movement, idle, [idle, walk, run, hit, die], transitions);

        // State Info Display
        // GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
    }


    private void UpdateFacing(float direction)
    {
        if (!Mathf.IsZeroApprox(direction))
            // 修改朝向逻辑：direction < 0 时朝左（-1），direction > 0 时朝右（1）
            Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
    }

    private void Patrol(double delta)
    {
        // 检查是否碰到墙壁
        if (_wallChecker.IsColliding())
            // 转向：将 X 缩放在 1 和 -1 之间切换
            Graphics.Scale = new Vector2(Graphics.Scale.X * -1, 1);

        // 移动逻辑
        var velocity = Velocity;
        velocity.X = Agent.Attr("WalkSpeed") * -Graphics.Scale.X; // 注意这里加了负号
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Velocity = velocity;
        MoveAndSlide();
    }

    private void Chase(double delta)
    {
        if (!_playerChecker.IsColliding()) return;

        var playerPosition = (_playerChecker.GetCollider() as Node2D)?.GlobalPosition;
        if (!playerPosition.HasValue) return;
        
        var direction = (playerPosition.Value - GlobalPosition).Normalized();
        var velocity = Velocity;
        velocity.X = direction.X * Agent.Attr("RunSpeed");
        velocity.Y += (float)delta * Agent.Attr("Gravity");
        Velocity = velocity;
        // 修改朝向：当向左移动时 Scale.X = -1，向右移动时 Scale.X = 1
        Graphics.Scale = new Vector2(direction.X >= 0 ? -1 : 1, 1);
        MoveAndSlide();
    }
}