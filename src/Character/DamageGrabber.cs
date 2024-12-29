using Miros.Core;

namespace BraveStory;

public class DamageGrabber : Grabber
{
    public override Tag AttributeTag { get; protected set; } = Tags.Attribute_HP;
    public override ModifierOperation Operation { get; protected set; } = ModifierOperation.Minus;
    public override GrabType GrabType { get; protected set; } = GrabType.Source;

    public override void Rewrite(Effect effect, Modifier modifier)
    {
        var targetDefense = effect.OwnerAgent.Atr("Defense");
        var magnitude = modifier.Magnitude;

        modifier.Magnitude = magnitude - targetDefense;
    }
}