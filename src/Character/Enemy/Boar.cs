using System;
using BraveStory;
using Godot;
using Miros.Core;

public partial class Enemy : Character
{
    private RayCast2D _floorChecker;
    private RayCast2D _playerChecker;
    private RayCast2D _wallChecker;

    private float _knockbackVelocity = 50.0f;
    private bool _isStunned = false;
    private float _stunDuration = 1.0f;
    private float _stunTimer = 0.0f;
    private float _chargeDuration = 0.5f;  // 冲刺持续时间
    private float _chargeTimer = 0f;       // 冲刺计时器
    private bool _isCharging = false;      // 是否正在冲刺


    public override void _Ready()
    {
        base._Ready();
        // Components
        _wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        _floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        _playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");

        
        // 设置初始朝向为左边
        Graphics.Scale = new Vector2(-1, 1);


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