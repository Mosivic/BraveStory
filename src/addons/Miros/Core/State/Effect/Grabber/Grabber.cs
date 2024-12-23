using Miros.Core;

public abstract class Grabber
{
    protected  abstract Tag AttributeSetTag {get;set;}
    protected  abstract Tag AttributeTag{get;set;}
    protected  abstract ModifierOperation Operation{get;set;}
    protected abstract GrabType GrabType {get;set;}

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