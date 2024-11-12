
namespace Miros.Core;

public abstract class Cue: State
{
    public TagSet RequiredTags;
    public TagSet ImmunityTags;
    protected Effect SourceEffect;
    protected Ability SourceAbility;
    protected object[] CustomArguments;
}

