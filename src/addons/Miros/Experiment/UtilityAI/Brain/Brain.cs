using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Experiment.UtilityAI;

public partial class Brain : Node2D
{
    public List<ActionBase> actions = [];
    public Context context;

    public override void _Ready()
    {
        context = new Context(this);

        foreach (var action in actions)
            action.Init(context);
    }


    public override void _Process(double delta)
    {
        UpdateContext();

        ActionBase bestAction = null;
        float bestUtility = float.MinValue;

        foreach (var action in actions)
        {
            var utility = action.CalculateUtility(context);
            if (utility > bestUtility)
            {
                bestUtility = utility;
                bestAction = action;
            }
        }

        bestAction?.Execute(context);
    }

    public void UpdateContext()
    {
        // TODO: 更新上下文
    }
}
