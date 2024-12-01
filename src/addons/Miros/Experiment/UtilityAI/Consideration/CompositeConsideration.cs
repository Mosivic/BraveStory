using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Experiment.UtilityAI;

public class CompositeConsideration : Consideration
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Max,
        Min,
        Average,
        Sum
    }

    public List<Consideration> considerations = [];

    public OperationType operationType = OperationType.Max;


    public override float Evaluate(Context context)
    {
        if (considerations == null || considerations.Count == 0)
            return 0f;

        var result = 0f;
        foreach (var consideration in considerations)
            result = operationType switch
            {
                OperationType.Max => Mathf.Max(result, consideration.Evaluate(context)),
                OperationType.Add => result + consideration.Evaluate(context),
                OperationType.Subtract => result - consideration.Evaluate(context),
                OperationType.Multiply => result * consideration.Evaluate(context),
                OperationType.Divide => result / consideration.Evaluate(context),
                OperationType.Min => Mathf.Min(result, consideration.Evaluate(context)),
                OperationType.Average => (result + consideration.Evaluate(context)) / 2,
                OperationType.Sum => result + consideration.Evaluate(context),
                _ => throw new ArgumentException($"Unsupported operation type: {operationType}")
            };

        return Mathf.Clamp(result, 0f, 1f);
    }
}