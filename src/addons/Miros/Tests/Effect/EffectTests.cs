using Godot;
using Miros.Core;
using NUnit.Framework;

namespace Miros.Tests;

[TestFixture]
public partial class EffectTests : Node2D
{
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
        var durationEffect = new Effect(Tags.Effect_Buff,_agent)
        {
            DurationPolicy = DurationPolicy.Duration,
            Duration = 10,
            Modifiers =
            [
                new Modifier(Tags.AttributeSet_Player, Tags.Attribute_RunSpeed, 10, ModifierOperation.Add)
            ]
        };
        

        var periodEffect = new Effect(Tags.Effect_Buff,_agent)
        {
            DurationPolicy = DurationPolicy.Period,
            Duration = 10,
            Period = 1,
            Modifiers =
            [
                new Modifier(Tags.AttributeSet_Player, Tags.Attribute_RunSpeed, 10, ModifierOperation.Add)
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
}