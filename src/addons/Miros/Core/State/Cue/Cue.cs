namespace Miros.Core;

public abstract class Cue(Tag sign, Agent source) : State(sign,source)
{
    protected object[] CustomArguments;
    public TagSet ImmunityTags;
    public TagSet RequiredTags;
    protected Effect SourceEffect;
}