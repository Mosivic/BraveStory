using System;
using System.Collections.Generic;
using Godot;
using Miros.Core;

namespace BraveStory;


public class CharacterShared : Shared
{
    public bool IsHit { get; set; } = false;
    public bool IsHurt { get; set; } = false;
    public AgentorBase HitAgentor { get; set; }
}

public partial class Character<TAgentor, TShared,TAttributeSet> : CharacterBody2D
where TAgentor : AgentorBase,new()
where TShared : CharacterShared, new()
where TAttributeSet : AttributeSet,new()
{
    protected AnimationPlayer AnimationPlayer;
    public Node2D Graphics;
    protected bool Hurt;
    protected HitBox HitBox;
    protected HurtBox HurtBox;
    protected Sprite2D Sprite;
    public StatsPanel StatusPanel;
    
    protected TAgentor Agentor;
    protected TShared Shared;


    public override void _Ready()
    {
        // 获取功能子节点
        Graphics = GetNode<Node2D>("Graphics");
        Sprite = Graphics.GetNode<Sprite2D>("Sprite2D");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        StatusPanel = GetNode<StatsPanel>("CanvasLayer/StatusPanel");
        
        // 处理击中事件 
        HitBox = Graphics.GetNode<HitBox>("HitBox");
        HitBox.OnHit += HandleHit;

        // 处理受伤事件
        HurtBox = Graphics.GetNode<HurtBox>("HurtBox");
        HurtBox.OnHurt += HandleHurt;

        // 初始化 Agentor
        Shared = new TShared();
        Agentor = new TAgentor();
        Agentor.Initialize<Character<TAgentor, TShared, TAttributeSet>,TAttributeSet>(this);

        // 处理伤害事件
        Agentor.Throttle<DamageSlice>("Damage", CreateDamageNumber);
    }


    public override void _ExitTree()
    {
        if (HitBox != null)
            HitBox.OnHit -= HandleHit;
        if (HurtBox != null)
            HurtBox.OnHurt -= HandleHurt;

        Agentor.Unthrottle<DamageSlice>("Damage", CreateDamageNumber);

        base._ExitTree();
    }


    public bool IsAnimationFinished()
    {
        return !AnimationPlayer.IsPlaying() && AnimationPlayer.GetQueue().Length == 0;
    }

    private void CreateDamageNumber(object sender, DamageSlice e)
    {
        var damageNumberScene = GD.Load<PackedScene>("res://Character/DamageNumber.tscn");
        var damageNumber = damageNumberScene.Instantiate<DamageNumber>();
        AddChild(damageNumber);
        
        damageNumber.SetDamage((int)e.Damage); 
    }

    public void PlayAnimation(string animationName)
    {
        AnimationPlayer.Play("RESET");
        AnimationPlayer.Play(animationName);
    }

    public string GetCurrentAnimation()
    {
        return AnimationPlayer.CurrentAnimation;
    }

    protected virtual void HandleHurt(object sender, HurtEventArgs e)
    {
        Shared.IsHurt = true;
    }

    protected virtual void HandleHit(object sender, HitEventArgs e)
    {
        Shared.IsHit = true;
        Shared.HitAgentor = (e.HurtBox.Owner as Character<TAgentor, TShared, TAttributeSet>).Agentor;
    }

    public override void _Process(double delta)
    {
        Agentor.Process(delta);
    }

}

