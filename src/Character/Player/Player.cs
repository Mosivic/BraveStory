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


	public void ClearInteractions()
	{
		Interactions.Clear();
	}

	public void SetHurtBoxMonitorable(bool monitorable)
	{
		HurtBox.SetDeferred("monitorable", monitorable);
	}

	public override void _Ready()
	{
		base._Ready();
		// Compoents
		_handChecker = GetNode<RayCast2D>("Graphics/HandChecker");
		_footChecker = GetNode<RayCast2D>("Graphics/FootChecker");
		_animatedSprite = GetNode<AnimatedSprite2D>("InteractionIcon");



		// 转换规则
		var transitions = new StateTransitionConfig();
		transitions
			.Add(idle, run, KeyDownMove) // 如果按下移动键，则进入跑动状态
			.Add(idle, fall, () => !IsOnFloor()) // 如果不在地板上，则进入坠落状态
			.Add(idle, jump, KeyDownJump) // 如果按下跳跃键，则进入跳跃状态
			.Add(idle, attack1, KeyDownAttack) // 如果按下攻击键，则进入攻击1状态
			.Add(idle, sliding, KeyDownSliding) // 如果按下滑动键，则进入滑行状态
			.Add(attack1, idle) // 攻击1状态结束后，进入空闲状态
			.Add(attack1, attack11, () => KeyDownAttack(), StateTransitionMode.DelayFront) // 如果攻击1过程中按下攻击键，则等待攻击1结束进入攻击11状态
			.Add(attack11, idle) // 攻击11状态结束后，进入空闲状态
			.Add(attack11, attack111, () => KeyDownAttack(),StateTransitionMode.DelayFront) // 如果攻击11过程中按下攻击键，则等待攻击11结束进入攻击111状态
			.Add(attack111, idle) // 攻击111状态结束后，进入空闲状态
			.Add(run, idle, () => !KeyDownMove()) // 如果未按下移动键，则进入空闲状态
			.Add(run, jump, KeyDownJump) // 如果按下跳跃键，则进入跳跃状态
			.Add(run, attack1, KeyDownAttack) // 如果按下攻击键，则进入攻击1状态
			.Add(run, sliding, KeyDownSliding) // 如果按下滑动键，则进入滑行状态
			.Add(jump, fall) // 跳跃状态结束后，进入坠落状态
			.Add(fall, idle, IsOnFloor) // 如果在地面上，则进入空闲状态
			.Add(fall, wallSlide, () => _footChecker.IsColliding() && _handChecker.IsColliding() && !KeyDownMove()) // 如果脚部检查器未碰撞到墙壁，则进入墙壁滑行状态
			.Add(fall, doubleJump, () => KeyDownJump() && _jumpCount < _maxJumpCount) // 如果按下跳跃键，并且跳跃次数小于最大跳跃次数，则进入双跳跃状态
			.Add(doubleJump, fall) // 双跳跃状态结束后，进入坠落状态
			.Add(wallSlide, idle, IsOnFloor) // 墙壁滑行状态结束后，如果在地面上，则进入空闲状态
			.Add(wallSlide, fall, () => !_footChecker.IsColliding()) // 如果脚部检查器未碰撞到墙壁，则进入坠落状态
			.Add(wallSlide, wallJump, KeyDownJump) // 如果按下跳跃键，则进入墙壁跳跃状态
			.Add(wallJump, fall) // 墙壁跳跃状态结束后，进入坠落状态
			.Add(hit, idle, IsAnimationFinished) // 如果动画播放完毕，则进入空闲状态
			.Add(sliding, idle, () => IsAnimationFinished()) // 滑行状态结束后，进入空闲状态
			.AddAny(hit, () => Hurt, StateTransitionMode.Force) // 如果受到伤害，则强制进入受伤状态
			.AddAny(die, () => Agent.Attr("HP") <= 0); // 如果生命值小于等于0，则进入死亡状态

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


	public void UpdateFacing(float direction)
	{
		// 如果有输入，根据输入方向转向
		if (!Mathf.IsZeroApprox(direction))
			Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
		// 如果没有输入，根据速度方向转向
		else if (!Mathf.IsZeroApprox(Velocity.X)) Graphics.Scale = new Vector2(Velocity.X < 0 ? -1 : 1, 1);
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



}
