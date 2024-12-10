using Godot;
using Miros.Core;

namespace BraveStory;

public partial class Character : CharacterBody2D
{
    protected Agent Agent;

    protected AnimationPlayer AnimationPlayer;
    public Node2D Graphics;
    protected bool Hurt;
    protected HitBox HitBox;
    protected HurtBox HurtBox;
    protected Sprite2D Sprite;


    public override void _Ready()
    {
        Graphics = GetNode<Node2D>("Graphics");
        Sprite = Graphics.GetNode<Sprite2D>("Sprite2D");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Agent = GetNode<Agent>("Agent");

        if(Agent == null)
        {
            GD.PrintErr($"[{Name}] Agent not found");
            return;
        }

        // 处理击中事件 
        HitBox = Graphics.GetNode<HitBox>("HitBox");
        HitBox.OnHit += HandleHit;

        // 处理受伤事件
        HurtBox = Graphics.GetNode<HurtBox>("HurtBox");
        HurtBox.OnHurt += HandleHurt;

        // 处理伤害事件
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


    public bool IsAnimationFinished()
    {
        return !AnimationPlayer.IsPlaying() && AnimationPlayer.GetQueue().Length == 0;
    }

    private void HandleDamage(object sender, DamageSlice e)
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
        Hurt = true;
    }

    protected virtual void HandleHit(object sender, HitEventArgs e)
    {
        var suffer = e.HurtBox.Owner as Character;

        var damageEffect = new Effect(Tags.Effect_Buff, Agent)
        {
            DurationPolicy = DurationPolicy.Instant,
            Executions = [
                new DamageExecution()
            ]
        };

        suffer.Agent.AddState(ExecutorType.EffectExecutor, damageEffect);
    }
}