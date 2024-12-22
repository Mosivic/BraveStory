using Miros.Core;

namespace BraveStory;

public class DamageGrabber : Grabber
{
    protected override Tag AttributeSetTag {get;set;}
    protected override Tag AttributeTag {get;set;} = Tags.Attribute_HP;
    protected override ModifierOperation Operation {get;set;} = ModifierOperation.Minus;
    protected override GrabType GrabType {get;set;} = GrabType.Source;

    public override void Rewrite(Effect effect, Modifier modifier)
    {
        var targetDefense = effect.OwnerAgent.Atr("Defense");
        var magnitude = modifier.Magnitude;

        modifier.Magnitude = magnitude - targetDefense;
    }
}