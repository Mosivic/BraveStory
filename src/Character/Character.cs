using Godot;
using Miros.Core;
using Miros.EventBus;

namespace BraveStory;

public partial class Character : CharacterBody2D
{
    protected Agent Agent;

    protected AnimationPlayer AnimationPlayer;
    protected Node2D Graphics;
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
        Hurt = true;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        EventBus.Instance.Subscribe<DamagedEventArgs>("Damaged", CueDamageNumber);
    }

    public override void _ExitTree()
    {
        EventBus.Instance.Unsubscribe<DamagedEventArgs>("Damaged", CueDamageNumber);
        if (HitBox != null)
            HitBox.OnHit -= HandleHit;
        if (HurtBox != null)
            HurtBox.OnHurt -= HandleHurt;

        base._ExitTree();
    }

    private void CueDamageNumber(object sender, DamagedEventArgs e)
    {
        if (e.Target != Agent)
            return;

        // 创建伤害数字实例
        var damageNumberScene = GD.Load<PackedScene>("res://Character/DamageNumber.tscn");
        var damageNumber = damageNumberScene.Instantiate<DamageNumber>();
        AddChild(damageNumber);
        
        // 设置伤害数值和位置
        damageNumber.SetDamage((int)e.Damage); // 这里的伤害值应该从 e 中获取
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