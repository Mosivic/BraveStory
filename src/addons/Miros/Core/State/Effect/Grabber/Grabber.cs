using Miros.Core;

public abstract class Grabber
{
    public  abstract Tag AttributeSetTag {get; protected set;}
    public  abstract Tag AttributeTag{get; protected set;}
    public  abstract ModifierOperation Operation{get;protected set;}
    public  abstract GrabType GrabType {get;protected set;}

    public Grabber(){}

    public Grabber(Tag attributeSetTag, Tag attributeTag,ModifierOperation operation)
    {
        AttributeSetTag = attributeSetTag;
        AttributeTag = attributeTag;
        Operation = operation;
    }
    
    public bool IsMatch(Modifier modifier)
    {
        return modifier.AttributeSetTag == AttributeSetTag && modifier.AttributeTag == AttributeTag && modifier.Operation == Operation;
    }

    public abstract void Rewrite(Effect effect,Modifier modifier);
}