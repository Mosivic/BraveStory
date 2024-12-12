using System;
using BraveStory;
using Godot;
using Miros.Core;

public class EnemyContext : CharacterContext
{
    public float KnockbackVelocity{get;set;} = 50.0f;
    public bool IsStunned{get;set;} = false;
    public float StunDuration{get;set;} = 1.0f;
    public float StunTimer{get;set;} = 0.0f;
    public float ChargeDuration{get;set;} = 0.5f;  // 冲刺持续时间
    public float ChargeTimer{get;set;} = 0f;       // 冲刺计时器
    public bool IsCharging{get;set;} = false;      // 是否正在冲刺
}

public partial class Enemy : Character
{
    private RayCast2D _floorChecker;
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

        Context = new EnemyContext();

        // 初始化 Agentor
        Agent.SetAttributeSet(typeof(BoarAttributeSet));
        Agent.AddTasksFromType<State,Enemy,EnemyContext>(this,Context as EnemyContext, [
            typeof(IdleEnemyAction), typeof(PatrolEnemyAction), typeof(DieEnemyAction), 
            typeof(ChargeEnemyAction), typeof(HitEnemyAction), typeof(StunEnemyAction)]);     

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

    public bool IsWallColliding() => _wallChecker.IsColliding();
    public bool IsFloorColliding() => _floorChecker.IsColliding();
    public bool IsPlayerColliding() => _playerChecker.IsColliding();
    public Player GetPlayer() => _playerChecker.GetCollider() as Player;
}