using Miros.Core;
using NUnit.Framework;

namespace Miros.Tests;

[TestFixture]
public class EffectTests
{
    private Agent _agent;
    private int _frameCounter;

    [SetUp]
    public void Setup()
    {
        _agent = new Agent(null, new StaticTaskProvider());
        _frameCounter = 0;
    }

    [Test]
    public void TestEffectInit()
    {
        var tag = new Tag("TestTag");
        var effect = new Effect(tag);

        Assert.That(effect.Tag, Is.EqualTo(tag));
    }

    [Test]
    public void TestEffectApply()
    {
        var tag = new Tag("Effect.Test");
        var effect = new Effect(tag)
        {
            Duration = 10,
            Period = 1,

            Modifiers =
            [
                new Modifier(Tags.Attribute_RunSpeed, Tags.Attribute_RunSpeed, 10, ModifierOperation.Add)
            ]
        };

        var attributeSet = new PlayerAttributeSet();
        attributeSet[Tags.Attribute_RunSpeed].SetBaseValue(100);

        var agent = new Agent(null, new StaticTaskProvider());
        agent.AttributeSetContainer.AddAttributeSet<PlayerAttributeSet>();


        Assert.That(attributeSet[Tags.Attribute_RunSpeed].BaseValue, Is.EqualTo(100));
    }
}