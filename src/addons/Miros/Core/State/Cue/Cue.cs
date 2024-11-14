namespace Miros.Core;

public abstract class Cue(string name, Tag sign) : State(name, sign)
{
    protected object[] CustomArguments;
    public TagSet ImmunityTags;
    public TagSet RequiredTags;
    protected Effect SourceEffect;
}