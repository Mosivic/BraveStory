using Godot;
using Miros.Core;
using NUnit.Framework;

namespace Miros.Tests;

[TestFixture]
public partial class EffectTests : Node2D
{

    private Effect durationEffect;
    private Effect effect2;
    [SetUp]
    public override void _Ready()
    {
        base._Ready();
        _agent = new Agent(null, new StaticTaskProvider());

        TestEffectApply();
    }

    private Agent _agent;
    private int _frameCounter;

    [Test]
    public void TestEffectInit()
    {
        var tag = new Tag("TestTag");
        var effect = new Effect(tag, _agent);

        Assert.That(effect.Tag, Is.EqualTo(tag));
    }

    [Test]
    public void TestEffectApply()
    {
        durationEffect = new Effect(Tags.Effect_Buff,_agent)
        {
            DurationPolicy = DurationPolicy.Duration,
            Duration = 100,
            Stacking = new EffectStacking
            {
                GroupTag = Tags.Effect_Buff,
                StackingType = StackingType.AggregateByTarget,
            },
            Modifiers =
            [
                new Modifier(new (Tags.AttributeSet_Character_Player, Tags.Attribute_RunSpeed), 10, ModifierOperation.Add)
            ]
        };
        

        effect2 = new Effect(Tags.Effect_Debuff,_agent)
        {
            DurationPolicy = DurationPolicy.Duration,
            Duration = 100,
            Stacking = new EffectStacking
            {
                GroupTag = Tags.Effect_Buff,
                StackingType = StackingType.AggregateByTarget,
            },
            Modifiers =
            [
                new Modifier(new (Tags.AttributeSet_Character_Player, Tags.Attribute_RunSpeed), 10, ModifierOperation.Add)
            ]
        };

        var periodEffect = new Effect(Tags.Effect_Buff,_agent)
        {
            DurationPolicy = DurationPolicy.Duration,
            Duration = 10,
            Period = 1,
            Modifiers =
            [
                new Modifier(new (Tags.AttributeSet_Character_Player, Tags.Attribute_RunSpeed), 10, ModifierOperation.Add)
            ]
        };
        _agent.CreateEffectExecutor();
        _agent.AddState(ExecutorType.EffectExecutor, durationEffect);
        
        _agent.AddAttributeSet(typeof(PlayerAttributeSet));
    }


    public override void _Process(double delta)
    {
        _agent.Update(delta);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.J)
        {
            GD.Print("Add Duration Effect");
            _agent.AddState(ExecutorType.EffectExecutor, effect2);
        }
    }
}