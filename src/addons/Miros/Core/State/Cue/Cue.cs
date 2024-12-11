namespace Miros.Core;

public abstract class Cue : State
{
    public TagSet ImmunityTags;
    public TagSet RequiredTags;
    public Effect SourceEffect;
}