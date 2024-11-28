namespace Miros.Core;

public interface IAgent
{
    T AttrSet<T>() where T : AttributeSet;
}