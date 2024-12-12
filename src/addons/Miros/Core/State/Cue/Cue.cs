namespace Miros.Core;

public abstract class Cue : State
{
    protected object[] CustomArguments;
    public TagSet ImmunityTags;
    public TagSet RequiredTags;
    protected Effect SourceEffect;
}