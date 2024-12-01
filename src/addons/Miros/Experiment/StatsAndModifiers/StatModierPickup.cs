/*
 * StatModifierPickup.cs
 * usage: 添加到可拾取的物品的节点上，拾取该物品时添加修改器
 */

namespace Miros.Experiment.StatsAndModifiers;

public partial class StatModifierPickup : PickupNode
{
    private readonly float _duration = 10;
    private readonly IOperationStrategy _operationStrategy = new AddOperationStrategy(1);
    private readonly StatType _statType = StatType.Attack;
    private readonly int _value = 10;

    protected override void ApplyPickupEffect(EntityNode entityNode)
    {
        var factory = new StatModifierFactory(); // TODO: using ServiceLocator wrapper
        var modifier = factory.Create(_operationStrategy, _statType, _value, _duration);
        entityNode.Stats.Mediator.AddModifier(modifier);
    }
}