
namespace Miros.Core;

public abstract class Cue(string name, Tag sign) : State(name, sign)
{
    public TagSet RequiredTags;
    public TagSet ImmunityTags;
    protected Effect SourceEffect;
    protected Ability SourceAbility;
    protected object[] CustomArguments;
}

