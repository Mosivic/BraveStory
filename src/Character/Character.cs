using System;
using System.Collections.Generic;
using Godot;
using Miros.Core;

namespace BraveStory;


public class CharacterShared : Shared
{
    public bool IsHit { get; set; } = false;
    public bool IsHurt { get; set; } = false;
    public AgentNodeBase HitAgentNode { get; set; }
}

public partial class Character<TShared> : CharacterBody2D
where TShared : CharacterShared, new()
{
    protected AnimationPlayer AnimationPlayer;
    public Node2D Graphics;
    protected bool Hurt;
    protected HitBox HitBox;
    protected HurtBox HurtBox;
    protected Sprite2D Sprite;
    protected AgentNodeBase AgentNode;
    protected CharacterShared Shared;


    public override void _Ready()
    {
        // 获取功能子节点
        Graphics = GetNode<Node2D>("Graphics");
        Sprite = Graphics.GetNode<Sprite2D>("Sprite2D");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        
        // 获取 AgentNode
        AgentNode = GetNode<AgentNodeBase>("Agent");
        Shared = AgentNode.GetShared<CharacterShared>();

        // 处理击中事件 
        HitBox = Graphics.GetNode<HitBox>("HitBox");
        HitBox.OnHit += HandleHit;

        // 处理受伤事件
        HurtBox = Graphics.GetNode<HurtBox>("HurtBox");
        HurtBox.OnHurt += HandleHurt;

        // 处理伤害事件
        AgentNode.Throttle<DamageSlice>("Damage", CreateDamageNumber);
    }


    public override void _ExitTree()
    {
        if (HitBox != null)
            HitBox.OnHit -= HandleHit;
        if (HurtBox != null)
            HurtBox.OnHurt -= HandleHurt;

        AgentNode.Unthrottle<DamageSlice>("Damage", CreateDamageNumber);

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
        Shared.HitAgentNode = (e.HurtBox.Owner as Character<TShared>).AgentNode;
    }

}

