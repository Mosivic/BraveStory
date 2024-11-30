using Example;
using Miros.Core;
using NUnit.Framework;

namespace Miros.Tests;

public class EffectTests
{
    [Test]
    public void TestEffectInit()
    {
        var tag = new Tag("TestTag");
        var effect = new Effect(tag);

        Assert.That(effect.Sign, Is.EqualTo(tag));
    }

    [Test]
    public void TestEffectApply()
    {
        var tag = new Tag("TestTag");
        var effect = new Effect(tag)
        {
            Duration = 10,
            Period = 1,
        };

        var attributeSet = new PlayerAttributeSet();
        attributeSet[Tags.Attribute_RunSpeed].SetBaseValue(100);

        var agent = new Agent(null, new StaticTaskProvider());
        agent.AttributeSetContainer.AddAttributeSet<PlayerAttributeSet>();

        Assert.That(attributeSet[Tags.Attribute_RunSpeed].BaseValue, Is.EqualTo(100));
    }
}
