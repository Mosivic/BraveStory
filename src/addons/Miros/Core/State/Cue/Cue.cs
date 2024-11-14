namespace Miros.Core;

public abstract class Cue(Tag sign) : State(sign)
{
    protected object[] CustomArguments;
    public TagSet ImmunityTags;
    public TagSet RequiredTags;
    protected Effect SourceEffect;
}