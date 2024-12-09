using Miros.Core;
using Miros.EventBus;

namespace BraveStory;

public class DamagedEventArgs : GameEventArgs
{
    public int Damage { get; }
    public Agent Target { get; }
    
    public DamagedEventArgs(int damage, Agent target) : base("Damaged")
    {
        Damage = damage;
        Target = target;
    }
}