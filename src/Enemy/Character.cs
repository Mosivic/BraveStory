using Godot;
using Miros.Core;

namespace BraveStory;

public partial class Character : CharacterBody2D
{
    protected AnimationPlayer _animationPlayer;
    protected Node2D _graphics;
    protected Sprite2D _sprite;
    protected HitBox _hitBox;
    protected HurtBox _hurtBox;
    protected bool _hasHit = false;

	protected MultiLayerStateMachineConnect _connect;


    public override void _Ready()
    {
        _graphics = GetNode<Node2D>("Graphics");
        _sprite = _graphics.GetNode<Sprite2D>("Sprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    
        // 处理击中事件 
		_hitBox = _graphics.GetNode<HitBox>("HitBox");
		_hitBox.OnHit += HandleHit;

		// 处理受伤事件
		_hurtBox = _graphics.GetNode<HurtBox>("HurtBox");
		_hurtBox.OnHurt += HandleHurt;

    }


    public override void _Process(double delta)
	{
		_connect.Update(delta);
	}


	public override void _PhysicsProcess(double delta)
	{
		_connect.PhysicsUpdate(delta);
	}


	protected bool WaitOverTime(Tag layer, double time)
	{
		return _connect.GetCurrentStateTime(layer) > time;
	}


    protected bool IsAnimationFinished()
	{
		return !_animationPlayer.IsPlaying() && _animationPlayer.GetQueue().Length == 0;
	}

    protected void PlayAnimation(string animationName)
	{
		_animationPlayer.Play(animationName);
	}

    protected virtual void HandleHurt(object sender, HurtEventArgs e)
    {
		GD.Print($"[Hurt]{Name} Received buff :{e.Buff.Name}");
		_hasHit = true;
    }


    protected virtual void HandleHit(object sender, HitEventArgs e)
    {

    }


    public override void _ExitTree()
    {

        if (_hitBox != null)
            _hitBox.OnHit -= HandleHit;
        if (_hurtBox != null)
            _hurtBox.OnHurt -= HandleHurt;
        base._ExitTree();
    }
}

