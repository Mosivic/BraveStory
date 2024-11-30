namespace Miros.Experiment.StatsAndModifiers;

public enum StatType {Attack,Defence}

public class Stats(StatsMediator mediator, BaseStats baseStats)
{
    public StatsMediator Mediator => mediator;
    
    public int Attack
    {
        get
        {
            var q = new Query(StatType.Attack, baseStats.Attack);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    
    public int Defence
    {
        get
        {
            var q = new Query(StatType.Defence, baseStats.Defense);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    
    public override string ToString()=> $"Attack: {Attack} Defence: {Defence}";
}