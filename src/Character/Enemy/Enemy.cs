using BraveStory;
using Godot;
using Miros.Core;

public class EnemyContext(Enemy host) : CharacterContext
{
    public Enemy Host { get; } = host;
    public float KnockbackVelocity { get; set; } = 50.0f;
    public bool IsStunned { get; set; } = false;
    public float StunDuration { get; set; } = 1.0f;
    public float StunTimer { get; set; } = 0.0f;

    public bool IsStomping { get; set; } = false; // 是否正在踩踏
    public float StompDuration { get; set; } = 0.3f; // 踩踏持续时间
    public float StompTimer { get; set; } = 0f; // 踩踏计时器
}

public partial class Enemy : Character
{
    private RayCast2D _floorChecker;
    private RayCast2D _playerChecker;
    private RayCast2D _wallChecker;
    protected StatsPanel StatsPanel;


    public override void _Ready()
    {
        base._Ready();
        // Components
        _wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        _floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        _playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");
        StatsPanel = GetNode<StatsPanel>("StatusPanel");

        // 设置初始朝向为左边
        Graphics.Scale = new Vector2(-1, 1);

        Context = new EnemyContext(this);

        // 初始化 Agent
        Agent.AddState(new IdleEnemyActionState(), Context);
        Agent.AddState(new PatrolEnemyActionState(), Context);
        Agent.AddState(new DieEnemyActionState(), Context);
        Agent.AddState(new ChargeEnemyActionState(), Context);
        Agent.AddState(new HurtEnemyActionState(), Context);
        Agent.AddState(new StunEnemyActionState(), Context);
        Agent.AddState(new StompGroundEnemyActionState(), Context);

        Agent.AddAttributeSet(typeof(BoarAttributeSet));
        var hp = Agent.GetAttributeBase("HP");
        hp.SetMaxValue(hp.CurrentValue);
        hp.RegisterPostCurrentValueChange(StatsPanel.OnUpdateHealthBar);

        // State Info Display
        // GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
    }

    private void UpdateFacing(float direction)
    {
        if (!Mathf.IsZeroApprox(direction))
            // 修改朝向逻辑：direction < 0 时朝左（-1），direction > 0 时朝右（1）
            Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
    }

    public bool IsWallColliding()
    {
        return _wallChecker.IsColliding();
    }

    public bool IsFloorColliding()
    {
        return _floorChecker.IsColliding();
    }

    public bool IsPlayerColliding()
    {
        return _playerChecker.IsColliding();
    }

    public Player GetPlayer()
    {
        return _playerChecker.GetCollider() as Player;
    }
}