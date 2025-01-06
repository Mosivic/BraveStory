using Miros.Core;

public abstract class Grabber
{
    public Grabber()
    {
    }

    public Grabber(Tag attributeSetTag, Tag attributeTag, ModifierOperation operation)
    {
        AttributeSetTag = attributeSetTag;
        AttributeTag = attributeTag;
        Operation = operation;
    }

    public virtual Tag AttributeSetTag { get; protected set; } = Tags.Default;
    public virtual Tag AttributeTag { get; protected set; } = Tags.Default;
    public virtual ModifierOperation Operation { get; protected set; }
    public virtual GrabType GrabType { get; protected set; }

    public bool IsMatch(Modifier modifier)
    {
        return modifier.AttributeSetTag == AttributeSetTag && modifier.AttributeTag == AttributeTag &&
               modifier.Operation == Operation;
    }

    public abstract void Rewrite(Effect effect, Modifier modifier);
}