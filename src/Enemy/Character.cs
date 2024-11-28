using Godot;
using Miros.Core;

namespace BraveStory;

public partial class Character : CharacterBody2D
{
    protected AnimationPlayer AnimationPlayer;
    protected Node2D Graphics;
    protected bool HasHit;
    protected HitBox HitBox;
    protected HurtBox HurtBox;

    protected Agent Agent;
    protected Sprite2D Sprite;


    public override void _Ready()
    {
        Graphics = GetNode<Node2D>("Graphics");
        Sprite = Graphics.GetNode<Sprite2D>("Sprite2D");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        // 处理击中事件 
        HitBox = Graphics.GetNode<HitBox>("HitBox");
        HitBox.OnHit += HandleHit;

        // 处理受伤事件
        HurtBox = Graphics.GetNode<HurtBox>("HurtBox");
        HurtBox.OnHurt += HandleHurt;
    }


    public override void _Process(double delta)
    {
        Agent.Update(delta);
    }


    public override void _PhysicsProcess(double delta)
    {
        Agent.PhysicsUpdate(delta);
    }

    protected bool IsAnimationFinished()
    {
        return !AnimationPlayer.IsPlaying() && AnimationPlayer.GetQueue().Length == 0;
    }

    protected void PlayAnimation(string animationName)
    {
        AnimationPlayer.Play(animationName);
    }

    protected virtual void HandleHurt(object sender, HurtEventArgs e)
    {
        // GD.Print($"[Hurt]{Name} Received buff :{e.Buff.Name}");
        // _hasHit = true;
    }


    protected virtual void HandleHit(object sender, HitEventArgs e)
    {
    }


    public override void _ExitTree()
    {
        if (HitBox != null)
            HitBox.OnHit -= HandleHit;
        if (HurtBox != null)
            HurtBox.OnHurt -= HandleHurt;
        base._ExitTree();
    }
}