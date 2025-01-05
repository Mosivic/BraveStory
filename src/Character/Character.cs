using Godot;
using Miros.Core;

namespace BraveStory;

public class CharacterContext : Context
{
    public bool IsHit { get; set; }
    public bool IsHurt { get; set; }
    public Agent HitAgent { get; set; }
}

public partial class Character : CharacterBody2D
{
    protected Agent Agent = new();

    protected AnimationPlayer AnimationPlayer;
    protected CharacterContext Context;
    protected HitBox HitBox;
    protected HurtBox HurtBox;
    protected Sprite2D Sprite;
    public Node2D Graphics { get; private set; }


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

        // 处理伤害事件
        Agent.Init(this);
        Agent.EventStream.Throttle<DamageSlice>("Damage", HandleDamage);
    }


    public override void _ExitTree()
    {
        if (HitBox != null)
            HitBox.OnHit -= HandleHit;
        if (HurtBox != null)
            HurtBox.OnHurt -= HandleHurt;

        Agent.EventStream.Unthrottle<DamageSlice>("Damage", HandleDamage);

        base._ExitTree();
    }

    public string GetCurrentAnimation()
    {
        return AnimationPlayer.GetCurrentAnimation();
    }

    public bool IsAnimationFinished()
    {
        return !AnimationPlayer.IsPlaying();
    }

    public void PlayAnimation(string animationName)
    {
        AnimationPlayer.Play(animationName);
    }

    protected virtual void HandleHurt(object sender, HurtEventArgs e)
    {
        Context.IsHurt = true;

    }

    protected virtual void HandleHit(object sender, HitEventArgs e)
    {
        Context.IsHit = true;
        Context.HitAgent = (e.HurtBox.Owner as Character).Agent;
    }

    public override void _Process(double delta)
    {
        Agent.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        Agent.PhysicsProcess(delta);
    }
}
