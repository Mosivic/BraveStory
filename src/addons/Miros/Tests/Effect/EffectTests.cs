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
        _frameCounter = 0;

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
        var effect = new Effect(Tags.Effect_Buff,_agent)
        {
            Duration = 10,
            Period = 1,

            Modifiers =
            [
                new Modifier(Tags.AttributeSet_Player, Tags.Attribute_RunSpeed, 10, ModifierOperation.Add)
            ]
        };


        _agent.AttributeSetContainer.AddAttributeSet<PlayerAttributeSet>();

        _agent.CreateEffectExecutor();
        _agent.AddState(ExecutorType.EffectExecutor, effect);

        //Assert.That(attributeSet[Tags.Attribute_RunSpeed].BaseValue, Is.EqualTo(110));
    }


    [Test]
    [TestCase(0D)]
    public override void _Process(double delta)
    {
        _frameCounter++;
        _agent.Update(delta);
    }
}