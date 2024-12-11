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

    public bool IsWallColliding() => _wallChecker.IsColliding();
    
    public bool IsFloorColliding() => _floorChecker.IsColliding();
    public bool IsPlayerColliding() => _playerChecker.GetCollider() is Player;

    public override void _Ready()
    {
        base._Ready();
        // Components
        _wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        _floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        _playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");

        
        // 设置初始朝向为左边
        Graphics.Scale = new Vector2(-1, 1);



        // Stun
        var stun = new State(Tags.State_Action_Stun, Agent)
            .OnEntered(s => { 
                PlayAnimation("idle");
                _stunTimer = 0.0f;
                _isStunned = false;
            })
            .OnPhysicsUpdated((s, d) => {
                _stunTimer += (float)d;
            });

            
        // Transitions
        var transitions = new StateTransitionConfig();
        transitions
            .Add(idle, walk, () => !_playerChecker.IsColliding())
            .Add(idle, charge, () => _playerChecker.IsColliding())
            .Add(walk, idle, () =>
                (!_floorChecker.IsColliding() && !_playerChecker.IsColliding() && walk.RunningTime > 2) ||
                (!_floorChecker.IsColliding() && _playerChecker.IsColliding()))
            .Add(walk, charge, () => _playerChecker.IsColliding())
            .AddAny(hit, () => Hurt)
            .Add(hit, idle, IsAnimationFinished)
            .AddAny(die, () => Agent.Attr("HP") <= 0)
            .Add(charge, idle, () => _chargeTimer >= _chargeDuration) // 冲刺时间结束
            .Add(charge, stun, () => _isStunned) // 撞墙时转为晕眩状态
            .Add(stun, idle, () => _stunTimer >= _stunDuration); // 晕眩时间结束

        Agent.CreateMultiLayerStateMachine(Tags.StateLayer_Movement, idle, [idle, walk, hit, die, charge, stun], transitions);

        // State Info Display
        // GetNode<StateInfoDisplay>("StateInfoDisplay").Setup(_connect, Tags.LayerMovement);
    }

    private void UpdateFacing(float direction)
    {
        if (!Mathf.IsZeroApprox(direction))
            // 修改朝向逻辑：direction < 0 时朝左（-1），direction > 0 时朝右（1）
            Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
    }

}